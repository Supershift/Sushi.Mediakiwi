using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// This list shows a code lens editor with the Tiny MCE content
    /// </summary>
    public class CodePlus : BaseImplementation
    {
        /// <summary>
        /// Initializes a new codePlus window
        /// </summary>
        public CodePlus()
        {
            wim.OpenInEditMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;
            wim.Page.Head.EnableColorCodingLibrary = true;
            wim.HideOpenCloseToggle = true;

            this.ListLoad += CodePlus_ListLoad;
            
        }

        void CodePlus_ListLoad(object sender, ComponentListEventArgs e)
        { 
            wim.Page.Head.Add(string.Format(@"
                                <style>
                                html, body  {{
                                    height: 580px !important;
                                    overflow: hidden;
                                }}
         
                               .cmsable .CodeMirror {{
                                    width: 668px!important;
                                     height: 505px;
                                }}
                                .formTable {{
                                    height: 515px;
                                    overflow: auto;
                                     width: 690px!important;
                                }}
                            </style> 
                            "));
            string body = @"<div class=""cmsable"">
                            <table class=""formTable"">
                                <tbody>
                                    <tr>
                                        <td><textarea cols=""32"" rows=""8"" class=""long SourceCode editable"" name=""codePlus"" id=""codePlus""></textarea>
                                         </td>
                                    </tr>
                                </tbody> 
                            </table> 
                        </div>
                        <footer>
                            <span class=""""> </span>
                            <div>
                                <input id=""save"" class=""submit  action right"" type=""submit"" value=""Ok"">
                            </div>
                        </footer>
                        <script>  
                            CodeMirror.defaults.wordwrapping =  true;
                            CodeMirror.defaults.autoCloseTags = true;
                            CodeMirror.defaults.lineNumbers = true;
                            CodeMirror.defaults.fixedGutter = true;
                            CodeMirror.defaults.lineWrapping = true;
                            var args = parent.tinymce.activeEditor.windowManager.getParams();
         
                            $('#codePlus').text(args.arg1);

                            $('#save').bind('click', function () {
                                var editor = top.tinymce.activeEditor;
                                editor.focus();

                                editor.undoManager.transact(function ()  {
                                    var cm = $('.CodeMirror')[0].CodeMirror;
                                    var val = cm.getValue();
                                    editor.setContent(val);
                                });

                                editor.selection.setCursorLocation();
                                editor.nodeChanged();

                                editor.windowManager.close();
                            });

                        </script>
                ";
            wim.Page.Body.Add(body,true);
        } 
    }
}
