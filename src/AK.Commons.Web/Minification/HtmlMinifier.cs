/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.HtmlMinifier
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Web Library (AKCWL).
 *  
 * AKCWL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCWL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCWL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Composition;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// Minifies HTML content. This is a stripped-down, cleaned up version of Zeta Html Compressor that you can find at:
    /// http://blog.magerquark.de/c-port-of-googles-htmlcompressor-library/.
    /// 
    /// Carrying over further credits from that port:
    ///  Original von: https://code.google.com/p/htmlcompressor/
    ///  Diese Datei von https://code.google.com/p/htmlcompressor/source/browse/trunk/src/main/java/com/googlecode/htmlcompressor/compressor/HtmlCompressor.java
    ///  Tipps auf http://stackoverflow.com/questions/3789472/what-is-the-c-sharp-regex-equivalent-to-javas-appendreplacement-and-appendtail
    ///  Java-Regex auf http://www.devarticles.com/c/a/Java/Introduction-to-the-Javautilregex-Object-Model/8/
    /// </summary>
    [Export(typeof (IMinifier))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ProviderMetadata("html")]
    public class HtmlMinifier : IMinifier
    {
        #region Fields

        // Temporary replacement formats for preserved blocks.
        //
        private const string PreBlockPreserveFormat = "%%%~COMPRESS~PRE~{0}~%%%";
        private const string TextAreaBlockPreserveFormat = "%%%~COMPRESS~TEXTAREA~{0}~%%%";
        private const string ScriptBlockPreserveFormat = "%%%~COMPRESS~SCRIPT~{0}~%%%";
        private const string StyleBlockPreserveFormat = "%%%~COMPRESS~STYLE~{0}~%%%";
        private const string EventBlockPreserveFormat = "%%%~COMPRESS~EVENT~{0}~%%%";
        private const string SkipBlockPreserveFormat = "%%%~COMPRESS~SKIP~{0}~%%%";

        #region Compiled RegEx patterns

        // Compiled RegEx patterns.

        private static readonly Regex skipPattern =
            new Regex("<!--\\s*\\{\\{\\{\\s*-->(.*?)<!--\\s*\\}\\}\\}\\s*-->",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex commentPattern =
            new Regex("<!---->|<!--[^\\[].*?-->", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex multipleSpacePattern =
            new Regex("\\s+", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex tagEndSpacePattern =
            new Regex("(<(?:[^>]+?))(?:\\s+?)(/?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex
            tagLastUnquotedValuePattern = new Regex("=\\s*[a-z0-9-_]+$", RegexOptions.IgnoreCase);

        private static readonly Regex prePattern =
            new Regex("(<pre[^>]*?>)(.*?)(</pre>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex textAreaPattern =
            new Regex("(<textarea[^>]*?>)(.*?)(</textarea>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex scriptPattern =
            new Regex("(<script[^>]*?>)(.*?)(</script>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex stylePattern =
            new Regex("(<style[^>]*?>)(.*?)(</style>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex tagPropertyPattern =
            new Regex("(\\s\\w+)\\s*=\\s*(?=[^<]*?>)", RegexOptions.IgnoreCase);

        private static readonly Regex docTypePattern =
            new Regex("<!DOCTYPE[^>]*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex typeAttrPattern =
            new Regex("type\\s*=\\s*([\\\"']*)(.+?)\\1", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex jsTypeAttrPattern =
            new Regex("(<script[^>]*)type\\s*=\\s*([\"']*)(?:text|application)/javascript\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex jsLangAttrPattern =
            new Regex("(<script[^>]*)language\\s*=\\s*([\"']*)javascript\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex styleTypeAttrPattern =
            new Regex("(<style[^>]*)type\\s*=\\s*([\"']*)text/style\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex linkTypeAttrPattern =
            new Regex("(<link[^>]*)type\\s*=\\s*([\"']*)text/(?:css|plain)\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex linkRelAttrPattern =
            new Regex("<link(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?stylesheet\\1(?:[^>]*)>",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex formMethodAttrPattern =
            new Regex("(<form[^>]*)method\\s*=\\s*([\"']*)get\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex inputTypeAttrPattern =
            new Regex("(<input[^>]*)type\\s*=\\s*([\"']*)text\\2([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex booleanAttrPattern =
            new Regex("(<\\w+[^>]*)(checked|selected|disabled|readonly)\\s*=\\s*([\"']*)\\w*\\3([^>]*>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex httpProtocolPattern =
            new Regex("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])http:(//[^>]+?>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex httpsProtocolPattern =
            new Regex("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])https:(//[^>]+?>)",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex relExternalPattern =
            new Regex("<(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?external\\1(?:[^>]*)>",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex eventPattern1 =
            new Regex("(\\son[a-z]+\\s*=\\s*\")([^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*)(\")",
                      RegexOptions.IgnoreCase);

        private static readonly Regex eventPattern2 =
            new Regex("(\\son[a-z]+\\s*=\\s*')([^'\\\\\\r\\n]*(?:\\\\.[^'\\\\\\r\\n]*)*)(')", RegexOptions.IgnoreCase);

        #endregion Compiled RegEx patterns

        // Patterns for searching for temporary replacements.
        //
        private static readonly Regex prePreservePattern = new Regex("%%%~COMPRESS~PRE~(\\d+?)~%%%");
        private static readonly Regex textAreaPreservePattern = new Regex("%%%~COMPRESS~TEXTAREA~(\\d+?)~%%%");
        private static readonly Regex scriptPreservePattern = new Regex("%%%~COMPRESS~SCRIPT~(\\d+?)~%%%");
        private static readonly Regex stylePreservePattern = new Regex("%%%~COMPRESS~STYLE~(\\d+?)~%%%");
        private static readonly Regex eventPreservePattern = new Regex("%%%~COMPRESS~EVENT~(\\d+?)~%%%");
        private static readonly Regex skipPreservePattern = new Regex("%%%~COMPRESS~SKIP~(\\d+?)~%%%");

        #endregion Fields

        #region Methods (IMinifier)

        public string Minify(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return content;

            IList<string> preBlocks = new List<string>(),
                          textAreaBlocks = new List<string>(),
                          scriptBlocks = new List<string>(),
                          styleBlocks = new List<string>(),
                          eventBlocks = new List<string>(),
                          skipBlocks = new List<string>();

            content = PreserveBlocks(content, preBlocks, textAreaBlocks, scriptBlocks, styleBlocks,
                                     eventBlocks, skipBlocks);

            content = ProcessHtml(content);

            content = ReturnBlocks(content, preBlocks, textAreaBlocks, scriptBlocks, styleBlocks,
                                   eventBlocks, skipBlocks);

            return content;
        }

        public byte[] Minify(byte[] content, Encoding encoding)
        {
            return encoding.GetBytes(this.Minify(encoding.GetString(content)));
        }

        #endregion

        #region Methods (Private Helper)

        private static string PreserveBlocks(
            string html,
            ICollection<string> preBlocks,
            ICollection<string> textAreaBlocks,
            ICollection<string> scriptBlocks,
            ICollection<string> styleBlocks,
            ICollection<string> eventBlocks,
            ICollection<string> skipBlocks)
        {
            var matches = skipPattern.Matches(html);
            var index = 0;
            var lastValue = 0;
            var builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[1].Value.Trim().Length > 0))
            {
                skipBlocks.Add(match.Groups[1].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(string.Format(SkipBlockPreserveFormat, index++)));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = eventPattern1.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[2].Value.Trim().Length > 0))
            {
                eventBlocks.Add(match.Groups[2].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1" + string.Format(EventBlockPreserveFormat, index++) + "$3"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = eventPattern2.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[2].Value.Trim().Length > 0))
            {
                eventBlocks.Add(match.Groups[2].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1" + string.Format(EventBlockPreserveFormat, index++) + "$3"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = prePattern.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[2].Value.Trim().Length > 0))
            {
                preBlocks.Add(match.Groups[2].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1" + string.Format(PreBlockPreserveFormat, index++) + "$3"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = scriptPattern.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (Match match in matches)
            {
                // Ignore empty scripts.
                //
                if (match.Groups[2].Value.Trim().Length <= 0) continue;

                // Check type.
                //
                var type = string.Empty;
                var typeMatches = typeAttrPattern.Match(match.Groups[1].Value);
                if (typeMatches.Success) type = typeMatches.Groups[2].Value.ToLowerInvariant();

                if (type.Length == 0 || type.Equals("text/javascript") || type.Equals("application/javascript"))
                {
                    // Javascript block, preserve.
                    //
                    scriptBlocks.Add(match.Groups[2].Value);

                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1" + string.Format(ScriptBlockPreserveFormat, index++) + "$3"));

                    lastValue = match.Index + match.Length;
                }
                else if (!type.Equals("text/x-jquery-tmpl"))
                {
                    // If it is a JQuery template, ignore so it gets compressed with the rest of html.
                    // Otherwise, it is some custom script, preserve it inside "skip blocks".
                    //
                    skipBlocks.Add(match.Groups[2].Value);

                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1" + string.Format(SkipBlockPreserveFormat, index++) + "$3"));

                    lastValue = match.Index + match.Length;
                }
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = stylePattern.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[2].Value.Trim().Length > 0))
            {
                styleBlocks.Add(match.Groups[2].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1" + string.Format(StyleBlockPreserveFormat, index++) + "$3"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = textAreaPattern.Matches(html);
            index = lastValue = 0;
            builder = new StringBuilder();

            foreach (var match in matches.Cast<Match>().Where(x => x.Groups[2].Value.Trim().Length > 0))
            {
                textAreaBlocks.Add(match.Groups[2].Value);

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1" + string.Format(TextAreaBlockPreserveFormat, index++) + "$3"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            return builder.ToString();
        }

        private static string ReturnBlocks(
            string html,
            IList<string> preBlocks,
            IList<string> textAreaBlocks,
            IList<string> scriptBlocks,
            IList<string> styleBlocks,
            IList<string> eventBlocks,
            IList<string> skipBlocks)
        {
            var matches = textAreaPreservePattern.Matches(html);
            var builder = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (textAreaBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(textAreaBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = stylePreservePattern.Matches(html);
            builder = new StringBuilder();
            lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (styleBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(styleBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = scriptPreservePattern.Matches(html);
            builder = new StringBuilder();
            lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (scriptBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(scriptBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = prePreservePattern.Matches(html);
            builder = new StringBuilder();
            lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (preBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(preBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = eventPreservePattern.Matches(html);
            builder = new StringBuilder();
            lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (eventBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(eventBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            matches = skipPreservePattern.Matches(html);
            builder = new StringBuilder();
            lastValue = 0;

            foreach (Match match in matches)
            {
                var i = int.Parse(match.Groups[1].Value);
                if (skipBlocks.Count <= i) continue;

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result(skipBlocks[i]));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            html = builder.ToString();

            return html;
        }

        private static string ProcessHtml(string html)
        {
            html = RemoveCommentsFromHtml(html);
            html = SimplifyDocTypeInHtml(html);
            html = RemoveScriptAttributesFromHtml(html);
            html = RemoveStyleAttributesFromHtml(html);
            html = RemoveLinkAttributesFromHtml(html);
            html = RemoveFormAttributesFromHtml(html);
            html = RemoveInputAttributesFromHtml(html);
            html = SimplifyBooleanAttributesInHtml(html);
            html = RemoveHttpProtocolFromHtml(html);
            html = RemoveHttpsProtocolFromHtml(html);
            html = RemoveMultipleSpacesFromHtml(html);
            html = RemoveSpacesInsideTagsFromHtml(html);

            return html.Trim();
        }

        private static string RemoveSpacesInsideTagsFromHtml(string html)
        {
            // Remove spaces around equals sign inside tags
            //
            html = tagPropertyPattern.Replace(html, "$1=");

            // Remove ending spaces inside tags.

            var matches = tagEndSpacePattern.Matches(html);
            var builder = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matches)
            {
                // Keep space if attribute value is unquoted before trailing slash.
                //
                if (match.Groups[2].Value.StartsWith("/") && tagLastUnquotedValuePattern.IsMatch(match.Groups[1].Value))
                {
                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1 $2"));

                    lastValue = match.Index + match.Length;
                    continue;
                }
                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$1$2"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            return builder.ToString();
        }

        private static string RemoveMultipleSpacesFromHtml(string html)
        {
            return multipleSpacePattern.Replace(html, " ");
        }

        private static string RemoveCommentsFromHtml(string html)
        {
            return commentPattern.Replace(html, string.Empty);
        }

        private static string SimplifyDocTypeInHtml(string html)
        {
            return docTypePattern.Replace(html, "<!DOCTYPE html>");
        }

        private static string RemoveScriptAttributesFromHtml(string html)
        {
            // Remove type from script tags
            //
            html = jsTypeAttrPattern.Replace(html, "$1$3");

            // Remove language from script tags
            //
            html = jsLangAttrPattern.Replace(html, "$1$3");

            return html;
        }

        private static string RemoveStyleAttributesFromHtml(string html)
        {
            return styleTypeAttrPattern.Replace(html, "$1$3");
        }

        private static string RemoveLinkAttributesFromHtml(string html)
        {
            var matches = linkTypeAttrPattern.Matches(html);
            var builder = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matches)
            {
                if (Matches(linkRelAttrPattern, match.Groups[0].Value))
                {
                    // If "rel" = "stylesheet":

                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1$3"));

                    lastValue = match.Index + match.Length;
                    continue;
                }

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$0"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            return builder.ToString();
        }

        private static string RemoveFormAttributesFromHtml(string html)
        {
            return formMethodAttrPattern.Replace(html, "$1$3");
        }

        private static string RemoveInputAttributesFromHtml(string html)
        {
            return inputTypeAttrPattern.Replace(html, "$1$3");
        }

        private static string SimplifyBooleanAttributesInHtml(string html)
        {
            return booleanAttrPattern.Replace(html, "$1$2$4");
        }

        private static string RemoveHttpProtocolFromHtml(string html)
        {
            var matches = httpProtocolPattern.Matches(html);
            var builder = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matches)
            {
                if (!Matches(relExternalPattern, match.Groups[0].Value))
                {
                    // If "rel" not = "external"
                    //
                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1$2"));

                    lastValue = match.Index + match.Length;
                    continue;
                }
                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$0"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            return builder.ToString();
        }

        private static string RemoveHttpsProtocolFromHtml(string html)
        {
            var matches = httpsProtocolPattern.Matches(html);
            var builder = new StringBuilder();
            var lastValue = 0;

            foreach (Match match in matches)
            {
                if (!Matches(relExternalPattern, match.Groups[0].Value))
                {
                    // If "rel" not = "external"
                    //
                    builder.Append(html.Substring(lastValue, match.Index - lastValue));
                    builder.Append(match.Result("$1$2"));

                    lastValue = match.Index + match.Length;
                    continue;
                }

                builder.Append(html.Substring(lastValue, match.Index - lastValue));
                builder.Append(match.Result("$0"));

                lastValue = match.Index + match.Length;
            }

            builder.Append(html.Substring(lastValue));
            return builder.ToString();
        }

        private static bool Matches(Regex regex, string value)
        {
            // The following code courtesy of:
            // http://stackoverflow.com/questions/4450045/difference-between-matches-and-find-in-java-regex

            return new Regex(@"^" + regex + @"$", regex.Options).IsMatch(value);
        }

        #endregion
    }
}