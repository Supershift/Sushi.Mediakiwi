/**
 * CodePlus/plugin.js
 * Developed by Casper Broeren 
 */ 
/*global tinymce:true */

var currentEditor;
function getSource() {
    return currentEditor.getContent({ source_view: true });
}
function setSource(data) {
    currentEditor.focus();
    currentEditor.undoManager.transact(function () {
        currentEditor.setContent(data);
    });
    currentEditor.selection.setCursorLocation();
    currentEditor.nodeChanged();
}

tinymce.PluginManager.add('codePlus', function (editor) {
   
	function showDialog() {
	    var src = editor.getContent({ source_view: true });
	    var w = editor.windowManager.open({
	        title: "Source code v2",
	        width: editor.getParam("codePlus_dialog_width", 700),
	        height: editor.getParam("codePlus_dialog_height", 590),
	        // All functionality is stored in the codeEditor page (Mediakiwi\Wim.Applications\Implementation\CodePlus.cs)
	        url:   '?list=793C9C8B-8713-4556-AB68-D284F81E187F&openinframe=1&referid=tmce&notitle=1',
		 
	    },
        {
            arg1: src
        });
	}

	editor.addCommand("mceCodeEditor", showDialog);

	editor.addButton('codePlus', {
		icon: 'code',
		tooltip: 'Source code plus', 
		onclick: showDialog

	});

	editor.addMenuItem('codePlus', {
		icon: 'code',
		text: 'Source code plus',
		context: 'tools',
		onclick: showDialog
	});

 
});