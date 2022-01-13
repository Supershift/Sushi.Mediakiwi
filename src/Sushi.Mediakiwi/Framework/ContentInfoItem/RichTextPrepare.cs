using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class RichTextPrepare
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextPrepare"/> class.
        /// </summary>
        public RichTextPrepare()
        {
            ConvertTableHeadCellToTH = true;
            TableTag = "cellspacing=\"1\" class=\"dataTable\"";
            TableRowTagEven = "class=\"even\"";
            TableRowTagOdd = "class=\"odd\"";
            TableRowFirstCell = "class=\"firstChild\"";
        }

        /// <summary>
        /// Gets or sets the table tag.
        /// </summary>
        /// <value>The table tag.</value>
        public string TableTag { get; set; }
        /// <summary>
        /// Gets or sets the table row tag even.
        /// </summary>
        /// <value>The table row tag even.</value>
        public string TableRowTagEven { get; set; }
        /// <summary>
        /// Gets or sets the table row tag odd.
        /// </summary>
        /// <value>The table row tag odd.</value>
        public string TableRowTagOdd { get; set; }
        /// <summary>
        /// Gets or sets the table row first cell.
        /// </summary>
        /// <value>The table row first cell.</value>
        public string TableRowFirstCell { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [convert table head cell to TH].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [convert table head cell to TH]; otherwise, <c>false</c>.
        /// </value>
        public bool ConvertTableHeadCellToTH { get; set; }

        /// <summary>
        /// Cleans the specified candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        public string Clean(string candidate)
        {
            if (!string.IsNullOrEmpty(TableTag))
            {
                TableTag = string.Concat(" ", TableTag);
            }

            if (!string.IsNullOrEmpty(TableRowTagEven))
            {
                TableRowTagEven = string.Concat(" ", TableRowTagEven);
            }

            if (!string.IsNullOrEmpty(TableRowTagOdd))
            {
                TableRowTagOdd = string.Concat(" ", TableRowTagOdd);
            }

            if (!string.IsNullOrEmpty(TableRowFirstCell))
            {
                TableRowFirstCell = string.Concat(" ", TableRowFirstCell);
            }

            Regex tableFind = new Regex(@"(?<TEXT><table[^>]*>(.*?)</table>)", RegexOptions.IgnoreCase);
            return tableFind.Replace(candidate, ReplaceTable);
        }

        /// <summary>
        /// Replaces the table.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string ReplaceTable(Match m)
        {
            m_TableCellIndex = 0;
            m_TableRowGroupIndex = 0;
            m_TableRowGroupIndexMax = 0;
            m_TableRowIndex = 0;

            string candidate = m.Value;

            Regex table = new Regex("<(?=(table )).*?>", RegexOptions.IgnoreCase);

            candidate = candidate.Replace("<tbody>", string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
            candidate = candidate.Replace("</tbody>", string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
            candidate = table.Replace(candidate, ReplaceTableItem);

            Regex tableRow = new Regex(@"(?<TEXT><tr[^>]*>(.*?)</tr>)", RegexOptions.IgnoreCase);
            m_TableRowGroupIndexMax = tableRow.Match(candidate).Groups.Count;

            candidate = tableRow.Replace(candidate, ReplaceTableRowGroup);
            return candidate;
        }

        /// <summary>
        /// Replaces the table item.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string ReplaceTableItem(Match m)
        {
            return $"<table{TableTag}>";
        }

        int m_TableRowGroupIndex;
        int m_TableRowGroupIndexMax;
        /// <summary>
        /// Replaces the table row group.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string ReplaceTableRowGroup(Match m)
        {
            Regex tr = new Regex("<(?=(tr)).*?>", RegexOptions.IgnoreCase);
            Regex td = new Regex("<(?=(td)).*?>", RegexOptions.IgnoreCase);

            m_TableRowGroupIndex++;

            string candidate = tr.Replace(m.Value, ReplaceTableRow);
            candidate = td.Replace(candidate, ReplaceTableCell);

            if (m_TableRowGroupIndex == 1)
            {
                return $"<thead>{candidate}</thead>";
            }
            else if (m_TableRowGroupIndex == 2)
            {
                return $"<tbody>{candidate}";
            }
            else if (m_TableRowGroupIndex == 2 && m_TableRowGroupIndex == m_TableRowGroupIndexMax)
            {
                return $"<tbody>{candidate}</tbody>";
            }
            else if (m_TableRowGroupIndex == m_TableRowGroupIndexMax)
            {
                return $"{candidate}</tbody>";
            }
            else
            {
                return candidate;
            }
        }

        int m_TableRowIndex;
        /// <summary>
        /// Replaces the table row.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string ReplaceTableRow(Match m)
        {
            m_TableRowIndex++;

            if (m_TableRowIndex == 1)
            {
                return "<tr>";
            }
            
            if (System.Math.Abs(m_TableRowIndex % 2) == 1)
            {
                return $"<tr{TableRowTagEven}>";
            }
            else
            {
                return $"<tr{TableRowTagOdd}>";
            }
        }

        int m_TableCellIndex;
        /// <summary>
        /// Replaces the table cell.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private string ReplaceTableCell(Match m)
        {
            m_TableCellIndex++;

            string tag = "td";
            if (m_TableRowGroupIndex == 1 && ConvertTableHeadCellToTH)
            {
                tag = "th";
            }

            if (m_TableCellIndex == 1)
            {
                return $"<{tag}{TableRowFirstCell}>";
            }
            else
            {
                return $"<{tag}>";
            }
        }
    }
}
