using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestAutomation.Helper.Models;

namespace TestAutomation.Helper
{
    public static class ReportGenerator
    {
        
        public static string Generate(IEnumerable<IEnumerable<string>> data, string templatePath, string replaceText="[$TABLE$]")
        {
            string output = string.Empty;
            if(File.Exists(templatePath))
            {
                var templateHtml = File.ReadAllText(templatePath);
                if(!string.IsNullOrEmpty(templateHtml))
                {
                    output = templateHtml.Replace(replaceText, GenerateTableHtml(data));
                }
            }
            else
            {
                throw new Exception("Template file does not exist");
            }

            return output;
        }

        public static string GenerateElementHtml(string data, ReportElementType elementType, string url= "", string width="200px", string height="200px")
        {
            switch(elementType)
            {
                case ReportElementType.Text: return $"<span>{data}</span>";
                case ReportElementType.Link: return $"<a href=\"{url}\">{data}</a>";
                case ReportElementType.Image: return $"<img class=\"myImg\" src=\"{(url != null ? url.Replace("\\", "/") : "")}\" alt=\"{data}\" width=\"{width}\" height=\"{height}\"/>";
                case ReportElementType.Table: {
                        string html = string.Empty;

                        if (!string.IsNullOrEmpty(data) && data.IndexOf(",") != -1)
                        {
                            html += "<table><tbody>";
                            data.Split(',').ToList().ForEach(item =>
                            {
                                html += $"<tr><td>{item}</td></tr>";
                            });

                            html += "</tbody></table>";
                        }
                        else
                            html += $"<span>{data}</span>";

                        return html;
                    };
                default: return "";
            }
        }

        private static string GenerateTableHtml(IEnumerable<IEnumerable<string>> data)
        {
            string html = string.Empty;

            if (data != null && data.Count() > 1)
            {
                html += "<table class=\"table table-hover\">";
                var headerRowData = data.FirstOrDefault();
                if(data.All(a => a.Count() == headerRowData.Count()))
                {
                    if (headerRowData != null && headerRowData.Any())
                    {
                        html += "<thead><tr>";
                        headerRowData.ToList().ForEach(cell =>
                        {
                            html += $"<th>{cell}</th>";
                        });
                        html += "</tr></thead>";
                    }

                    html += "<tbody>";
                    data.Skip(1).ToList().ForEach(row =>
                    {
                        html += $"<tr class=\"{(row.Last().IndexOf("True") != -1 ? "table-success" : "table-danger")}\">";
                        row.ToList().ForEach(cell =>
                        {
                            html += $"<td>{cell}</td>";
                        });
                        html += "</tr>";
                    });
                    html += "</tbody>";
                }
                else
                {
                    throw new ArgumentException("All rows should be of same length.");
                }

                html += "</table>";
            }
            else
            {
                throw new ArgumentException("Data cannot be null or empty.");
            }

            return html;
        }
    }
}
