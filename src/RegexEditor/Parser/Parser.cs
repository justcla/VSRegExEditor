using System;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.RegularExpression.Coloring;

namespace Microsoft.VisualStudio.RegularExpression.Parser
{
    /// <summary>
    /// Parses the regular expression into tokens.
    /// </summary>
    internal sealed class Parser
    {
        internal IList<Token> Tokens { get; private set; }
        private int position;

        #region Language Constants
        
        private const char StartChar = '$';
        private const char EndChar = '^';
        private const char SpaceChar = ' ';
        private const char NewLineChar = '\n';
        private const char LineFeedChar = '\r';
        private const char TabChar = '\t';
        private const char StartCharGroupChar = '[';
        private const char EndCharGroupChar = ']';
        private const char StartRepetitionsChar = '{';
        private const char EndRepetitionsChar = '}';
        private const char StartCaptureChar = '(';
        private const char SecondCaptureChar = '?';
        private const char EndCaptureChar = ')';
        private const char EscapeChar = '\\';
        private const char HexExpressionChar = 'x';
        private const char UnicodeExpressionChar = 'u';
        private const char OctalExpressionChar = 'o';
        private const char OneOrManyChar = '+';
        private const char ZeroOrManyChar = '*';
        private const char ZeroOrOneChar = '?';
        private const char OrChar = '|';
        private const char NegativeCaptureModifierChar = '-';
        private const char CaptureSeparatorChar = ':';
        private const char NegativeChar = '!';
        private const char LessThanChar = '<';
        private const char EqualsChar = '=';
        private const char NamedCaptureEnd = '>';
        private const string ValidExpressionChars = "dDwWsSAzZbBuxovtrnf";
        private const string ValidCaptureModifiers = "imnsx";
        
        #endregion

        internal void Parse(string text)
        {
            this.Tokens = new List<Token>();
            this.position = 0;

            DropWhitespace(text);
            ParseStart(text);
            DropWhitespace(text);
            ParseBody(text, EndChar);
            DropWhitespace(text);
            ParseEnd(text);
        }

        //Whitespace needs not be parsed for coloring. Ignore it when needed.
        private void DropWhitespace(string text)
        {
            while (position < text.Length && (text[position] == SpaceChar || text[position] == NewLineChar || text[position] == LineFeedChar || text[position] == TabChar))
            {
                position++;
            }
        }

        //Parse the body of the expression, looking for different language parts and defaulting to plain text
        internal void ParseBody(string text, char until)
        {
            while (position < text.Length && text[position] != until)
            {
                if (!ParseCharGroup(text)
                    && !ParseRepetition(text)
                    && !ParseExpression(text)
                    && !ParseEscapedExpression(text)
                    && !ParseMultiplier(text)
                    && !ParseCapture(text))
                {
                    position++;
                }
                DropWhitespace(text);
            }
        }

        //Look for capture groups (of the form "(text)" in regular expressions)
        internal bool ParseCapture(string text)
        {
            if (position < text.Length && text[position] == StartCaptureChar)
            {
                int firstChar = position;
                position++;
                //When the first character in the group is a "?", there are many options for modifiers
                if (position < text.Length && text[position] == SecondCaptureChar)
                {
                    position++;
                    ParseModifiedCapture(text);
                }

                ParseBody(text, EndCaptureChar);
                if (position < text.Length)
                {
                    if (text[position] == EndCaptureChar)
                    {
                        position++;
                    }
                    Tokens.Add(new Token(firstChar, position, TokenKind.Capture, text[position - 1] == EndCaptureChar));
                }

                return true;
            }
            return false;
        }

        //Look for special patterns in a capture group (of the form "(?capture)" in regular expressions).
        private void ParseModifiedCapture(string text)
        {
            //Look for special modifiers to the capture. The modifiers are:
            // i for ignore case
            // m for multiline
            // n for explicit capture
            // s for single line
            // s for ignore whitespace
            if (position < text.Length && ValidCaptureModifiers.Contains(text[position].ToString()))
            {
                position++;
                if (position < text.Length && text[position] == CaptureSeparatorChar)
                {
                    position++;
                    Tokens.Add(new Token(position - 3, position, TokenKind.CaptureName));
                }
            }
            //Same as before, but prefixed with a '-' to turn off the setting
            else if (position < text.Length && text[position] == NegativeCaptureModifierChar)
            {
                position++;
                if (position < text.Length && ValidCaptureModifiers.Contains(text[position].ToString()))
                {
                    position++;
                    if (position < text.Length && text[position] == CaptureSeparatorChar)
                    {
                        position++;
                        Tokens.Add(new Token(position - 4, position, TokenKind.CaptureName));
                    }
                }
            }
            //Look for groups that aren't separate matches (of the form "(?:text)" in regular expressions)
            else if (position < text.Length && text[position] == CaptureSeparatorChar)
            {
                position++;
                Tokens.Add(new Token(position - 2, position, TokenKind.CaptureName));
            }
            //Look for lookahead expressions (of the form "(?=group)" in regular expressions)
            else if (position < text.Length && text[position] == EqualsChar)
            {
                position++;
                Tokens.Add(new Token(position - 2, position, TokenKind.CaptureName));
            }
            //Look for negative lookahead expressions (of the form "(?!group)" in regular expressions)
            else if (position < text.Length && text[position] == NegativeChar)
            {
                position++;
                Tokens.Add(new Token(position - 2, position, TokenKind.CaptureName));
            }
            else if (position < text.Length && text[position] == LessThanChar)
            {
                position++;
                ParseNamedGroup(text);
                position++;
            }
            //Look for conditional expressions (of the form "(?(text)yes|no)" in regular expressions)
            else if (position < text.Length && text[position] == StartCaptureChar)
            {
                int firstChar = position - 2;
                while (position < text.Length && text[position] != EndCaptureChar)
                {
                    position++;
                }

                if (position < text.Length)
                {
                    Tokens.Add(new Token(firstChar, position + 1, TokenKind.CaptureName, true));
                }
                else
                {
                    Tokens.Add(new Token(firstChar, position, TokenKind.CaptureName));
                }
                position++;
            }
        }

        //Look for named groups (represented as "(?<name>text)" in regular expressions). Up to this point, the starting "(?<" will be parsed.
        //The remainder might be of the form "(?<=", "(?<!" or an actual named group.
        private void ParseNamedGroup(string text)
        {
            //Look for lookbehind expressions (of the form "(?<=group)" in regular expressions)
            if (position < text.Length && text[position] == EqualsChar)
            {
                Tokens.Add(new Token(position - 2, position + 1, TokenKind.CaptureName));
            }
            //Look for negative lookbehind expressions (of the form "(?<!group)" in regular expressions)
            else if (position < text.Length && text[position] == NegativeChar)
            {
                Tokens.Add(new Token(position - 2, position + 1, TokenKind.CaptureName));
            }
            else
            {
                //Look for named groups
                int firstChar = position - 2;
                while (position < text.Length && text[position] != NamedCaptureEnd)
                {
                    position++;
                }

                if (position < text.Length)
                {
                    Tokens.Add(new Token(firstChar, position + 1, TokenKind.CaptureName, true));
                }
                else
                {
                    Tokens.Add(new Token(firstChar, position, TokenKind.CaptureName));
                }
            }
        }

        //Look for square brackets for character groups (represented as "[abcd]" in regular expressions)
        internal bool ParseCharGroup(string text)
        {
            if (position < text.Length && text[position] == StartCharGroupChar)
            {
                int firstChar = position;
                while (position < text.Length && text[position] != EndCharGroupChar)
                {
                    if (text[position] == EscapeChar)
                    {
                        position++;
                    }
                    if (position < text.Length)
                    {
                        position++;
                    }
                }
                if (position < text.Length)
                {
                    Tokens.Add(new Token(firstChar, position + 1, TokenKind.CharGroup, true));
                }
                else
                {
                    Tokens.Add(new Token(firstChar, position, TokenKind.CharGroup));
                }

                position++;
                return true;
            }
            return false;
        }

        //Look for escaped expressions (represented as "\" followed by anything but this characters: dDwWsSAzZbBuxo)
        internal bool ParseEscapedExpression(string text)
        {
            if (position < text.Length && text[position] == EscapeChar)
            {
                if (position + 1 < text.Length)
                {
                    Tokens.Add(new Token(position, position + 1, TokenKind.EscapedExpression));
                    position += 2;
                    return true;
                }
                else
                {
                    Tokens.Add(new Token(position, position, TokenKind.EscapedExpression));
                    position += 1;
                    return true;
                }
            }
            return false;
        }

        //Look for multipliers and separators ("|", "*", "+" and "?" in regular expressions)
        internal bool ParseMultiplier(string text)
        {//TODO: Change to position < length && char == '' || char == '' ...
            if (position < text.Length && text[position] == OneOrManyChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Multiplier));
                position++;
                return true;
            }
            else if (position < text.Length && text[position] == ZeroOrManyChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Multiplier));
                position++;
                return true;
            }
            else if (position < text.Length && text[position] == ZeroOrOneChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Multiplier));
                position++;
                return true;
            }
            else if (position < text.Length && text[position] == OrChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Multiplier));
                position++;
                return true;
            }
            return false;
        }

        //Look for escaped expressions (represented as "\" followed by any of this characters: dDwWsSAzZbBuxo)
        internal bool ParseExpression(string text)
        {
            if (position < text.Length && text[position] == EscapeChar)
            {
                if (position + 1 < text.Length && ValidExpressionChars.Contains(text[position + 1].ToString()))
                {
                    int result;
                    //Hexadecimal is represented by \x followed by two digits: \x00
                    if (text[position + 1] == HexExpressionChar)
                    {
                        if (position + 3 < text.Length && int.TryParse(text.Substring(position + 2, 2), out result))
                        {
                            Tokens.Add(new Token(position, position + 4, TokenKind.Expression));
                            position += 3;
                            return true;
                        }
                    }
                    //Unicode is represented by \u followed by four digits: \u0000
                    else if (text[position + 1] == UnicodeExpressionChar)
                    {
                        if (position + 5 < text.Length && int.TryParse(text.Substring(position + 2, 4), out result))
                        {
                            Tokens.Add(new Token(position, position + 6, TokenKind.Expression));
                            position += 5;
                            return true;
                        }
                    }
                    //Octal is represented by \o followed by three digits: \o000
                    else if (text[position + 1] == OctalExpressionChar)
                    {
                        if (position + 4 < text.Length && int.TryParse(text.Substring(position + 2, 3), out result))
                        {
                            Tokens.Add(new Token(position, position + 5, TokenKind.Expression));
                            position += 4;
                            return true;
                        }
                    }
                    //All other expressions are of the form \x, where x is one of the valid characters
                    Tokens.Add(new Token(position, position + 2, TokenKind.Expression));
                    position += 2;
                    return true;
                }
            }
            return false;
        }

        //Look for curly brackets for repetitions (represented as "{1}" in regular expressions)
        internal bool ParseRepetition(string text)
        {
            if (position < text.Length && text[position] == StartRepetitionsChar)
            {
                int firstChar = position;
                while (position < text.Length && text[position] != EndRepetitionsChar)
                {
                    position++;
                }
                if (position < text.Length)
                {
                    Tokens.Add(new Token(firstChar, position + 1, TokenKind.Repetition, true));
                }
                else
                {
                    Tokens.Add(new Token(firstChar, position, TokenKind.Repetition));
                }

                position++;
                return true;
            }
            return false;
        }

        //A regular expression may start with a "$" symbol
        internal bool ParseStart(string text)
        {
            if (position < text.Length && text[position] == StartChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Delimiter));
                position++;
                return true;
            }
            return false;
        }

        //A regular expression may end with a "^" symbol
        internal bool ParseEnd(string text)
        {
            if (position < text.Length && text[position] == EndChar)
            {
                Tokens.Add(new Token(position, position + 1, TokenKind.Delimiter));
                position++;
                return true;
            }
            return false;
        }
    }
}