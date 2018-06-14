using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Keeg.SharpBsaLib.Common
{
    public class NameFilter : INameFilter
    {
        #region Instance Fields
        private List<string> _inclusiveFilter = new List<string>();
        private List<string> _exclusiveFilter = new List<string>();
        private List<Regex> _inclusiveRegex = new List<Regex>();
        private List<Regex> _exclusiveRegex = new List<Regex>();
        #endregion

        #region Constructors
        /// <summary>
        /// Construct an instance based on the inclusive filter regular expression passed.
        /// </summary>
        /// <param name="inclusiveFilter">The inclusive regular expression filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NameFilter(string inclusiveFilter)
        {
            if (string.IsNullOrEmpty(inclusiveFilter))
            {
                throw new ArgumentNullException(nameof(inclusiveFilter));
            }

            _inclusiveFilter.Add(inclusiveFilter);

            Compile();
        }

        /// <summary>
        /// Constuct an instance based on the inclusive and exclusive regular expressions passed.
        /// </summary>
        /// <param name="inclusiveFilter">The inclusive regular expression filter.</param>
        /// <param name="exclusiveFilter">The exclusive regular expression filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NameFilter(string inclusiveFilter, string exclusiveFilter)
        {
            if (string.IsNullOrEmpty(inclusiveFilter))
            {
                throw new ArgumentNullException(nameof(inclusiveFilter));
            }
            if (string.IsNullOrEmpty(exclusiveFilter))
            {
                throw new ArgumentNullException(nameof(exclusiveFilter));
            }

            _inclusiveFilter.Add(inclusiveFilter);
            _exclusiveFilter.Add(exclusiveFilter);

            Compile();
        }

        /// <summary>
        /// Construct an instance based on the list of inclusive regular expressions passed.
        /// </summary>
        /// <param name="inclusiveFilters">The list of inclusive regular expressions to filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NameFilter(IEnumerable<string> inclusiveFilters)
        {
            if (inclusiveFilters == null)
            {
                throw new ArgumentNullException(nameof(inclusiveFilters));
            }

            foreach (var filter in inclusiveFilters)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    _inclusiveFilter.Add(filter);
                }
            }

            Compile();
        }

        /// <summary>
        /// Construct an instance based on the list of inclusive and exclusive regular expressions passed.
        /// </summary>
        /// <param name="inclusiveFilters">The list of inclusive regular expressions to filter.</param>
        /// <param name="exclusiveFilters">The list of exclusive regualr expressions to filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NameFilter(IEnumerable<string> inclusiveFilters, IEnumerable<string> exclusiveFilters)
        {
            if (inclusiveFilters == null)
            {
                throw new ArgumentNullException(nameof(inclusiveFilters));
            }
            if (exclusiveFilters == null)
            {
                throw new ArgumentNullException(nameof(exclusiveFilters));
            }

            foreach (var inFilter in inclusiveFilters)
            {
                if (!string.IsNullOrEmpty(inFilter))
                {
                    _inclusiveFilter.Add(inFilter);
                }
            }

            foreach (var exFilter in exclusiveFilters)
            {
                if (!string.IsNullOrEmpty(exFilter))
                {
                    _exclusiveFilter.Add(exFilter);
                }
            }

            Compile();
        }
        #endregion

        /// <summary>
        /// Compile and setup all the inclusive and exclusive regular expressions.
        /// </summary>
        /// <exception cref="ArgumentException">Can throw an exception from <see cref="Regex"/></exception>
        private void Compile()
        {
            foreach (var include in _inclusiveFilter)
            {
                _inclusiveRegex.Add(new Regex(include, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
            }

            foreach (var exclude in _exclusiveFilter)
            {
                _exclusiveRegex.Add(new Regex(exclude, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
            }
        }

        /// <summary>
        /// Tests a string to see if it's a valid regualr expression.
        /// </summary>
        /// <param name="expression">The string expression to test.</param>
        /// <returns>True if the expression is valid <see cref="Regex"/> otherwise false.</returns>
        public static bool IsValidExpression(string expression)
        {
            bool result = false;
            try
            {
                var regex = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                result = true;
            }
            catch (ArgumentException)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Test a value to see if it's excluded by the filter.
        /// </summary>
        /// <param name="name">The value to test for exclusion.</param>
        /// <returns>True if the value is excluded, otherwise false.</returns>
        public bool IsExcluded(string name)
        {
            bool result = false;
            if (_exclusiveRegex.Count == 0)
            {
                result = false;
            }
            else
            {
                foreach (var r in _exclusiveRegex)
                {
                    if (r.IsMatch(name))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Test a value to see if it's included by the filter.
        /// </summary>
        /// <param name="name">The value to test for inclusion.</param>
        /// <returns>True if the value is included, otherwise false.</returns>
        public bool IsIncluded(string name)
        {
            bool result = false;
            if (_inclusiveRegex.Count == 0)
            {
                result = true;
            }
            else
            {
                foreach (var r in _inclusiveRegex)
                {
                    if (r.IsMatch(name))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #region INameFilter Members
        /// <summary>
        /// Test a value to see if it's a match for the filter.
        /// </summary>
        /// <param name="name">The value to test.</param>
        /// <returns>True if the value matches, otherwise false.</returns>
        public bool IsMatch(string name)
        {
            return (IsIncluded(name) && !IsExcluded(name));
        }
        #endregion
    }
}
