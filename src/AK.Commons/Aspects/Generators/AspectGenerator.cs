/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.AspectGenerator
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library (AKCL).
 *  
 * AKCL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System.CodeDom;
using System.Reflection;
using System.Text;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Provides access to an common functionality for aspect code generators.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AspectGenerator
    {
        protected readonly MemberInfo memberInfo;

        private EntryAspectGenerator entry;
        private ExitAspectGenerator exit;
        private ErrorAspectGenerator error;

        public AspectGenerator(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public EntryAspectGenerator Entry
        {
            get { return this.entry ?? (this.entry = new EntryAspectGenerator(this.memberInfo)); }
        }

        public ExitAspectGenerator Exit
        {
            get { return this.exit ?? (this.exit = new ExitAspectGenerator(this.memberInfo)); }
        }

        public ErrorAspectGenerator Error
        {
            get { return this.error ?? (this.error = new ErrorAspectGenerator(this.memberInfo)); }
        }

        protected CodeExpression GenerateParameterDictionaryExpression(bool? isPropertyGet = false)
        {
            var parameterDictionarySnippet = "new System.Collections.Generic.Dictionary<string, object> {{ {0} }}";

            var sb = new StringBuilder();

            var methodInfo = this.memberInfo as MethodInfo;
            var propertyInfo = this.memberInfo as PropertyInfo;

            if (methodInfo != null)
            {
                foreach (var parameterInfo in methodInfo.GetParameters())
                    sb.AppendFormat("{{ \"{0}\", {0} }}, ", parameterInfo.Name);

                var index = 1;
                foreach (var type in methodInfo.GetGenericArguments())
                {
                    sb.AppendFormat("{{ \"T{0}\", typeof({1}) }}, ", index, type.Name);
                    index++;
                }
            }
            else if (propertyInfo != null)
            {
                foreach (var parameterInfo in propertyInfo.GetIndexParameters())
                    sb.AppendFormat("{{ \"{0}\", {0} }}, ", parameterInfo.Name);

                if (!(isPropertyGet ?? false)) sb.AppendFormat("{{ \"{0}\", {0} }}", "value");
            }

            parameterDictionarySnippet = string.Format(parameterDictionarySnippet, sb);

            return new CodeSnippetExpression(parameterDictionarySnippet);
        }
    }
}