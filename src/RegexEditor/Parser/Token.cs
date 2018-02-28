using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.RegularExpression.Coloring;

namespace Microsoft.VisualStudio.RegularExpression.Parser
{
    /// <summary>
    /// Unit of parse for a regular expression.
    /// </summary>
    internal struct Token
    {
        internal int Start;
        internal int End;
        internal TokenKind Kind;
        internal bool MatchBraces;

        internal Token(int start, int end, TokenKind kind)
        {
            Start = start;
            End = end;
            Kind = kind;
            MatchBraces = false;

        }

        internal Token(int start, int end, TokenKind kind, bool matchBraces)
        {
            Start = start;
            End = end;
            Kind = kind;
            MatchBraces = matchBraces;
        }

        internal static string TranslateEnumToString(TokenKind tokenKindEnum)
        {
            switch (tokenKindEnum)
            {
                case TokenKind.CharGroup:
                    return ClassificationTypes.CharGroup;
                case TokenKind.Repetition:
                    return ClassificationTypes.Repetition;
                case TokenKind.EscapedExpression:
                    return ClassificationTypes.EscapedExpression;
                case TokenKind.Expression:
                    return ClassificationTypes.Expression;
                case TokenKind.Multiplier:
                    return ClassificationTypes.Multiplier;
                case TokenKind.Delimiter:
                    return ClassificationTypes.Delimiter;
                case TokenKind.Capture:
                    return ClassificationTypes.Capture;
                case TokenKind.CaptureName:
                    return ClassificationTypes.Capture;
                default:
                    return "";
            }
        }
    }
}
