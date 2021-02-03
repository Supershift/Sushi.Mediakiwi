/*
	Method for controlling displayed components based on the url's query parameters.
*/

	function filterComponents() {
		// get the list of required components from the url
		componentsList = (document.location.href.indexOf('components=')>-1) ?
			document.location.href.split('components=')[1].split('#')[0].split('&')[0].split(',') :
			(parent.location.href.indexOf('components=')>-1) ?
				parent.location.href.split('components=')[1].split('#')[0].split('&')[0].split(',') :
				new Array() ;
		// build and inline stylesheet
		componentStyles = '<style>\n';
		if(componentsList.length>0)
			componentStyles += '.component {display : none !important;}\n';
		// for all components in the list
		for(var a=0; a<componentsList.length; a++)
			componentStyles += '#' + componentsList[a] + ', .component.' + componentsList[a] + ' {display : block !important;}\n';
		componentStyles += '</style>\n';
		// write the composed styles
		document.writeln(componentStyles);
	}

	filterComponents();

