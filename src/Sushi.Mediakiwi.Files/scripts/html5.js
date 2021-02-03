/*
	Method for defining HTML 5 elements in Microsoft Internet Explorer browsers.
*/

	/* HTML 5 - http://dev.w3.org/html5/spec/Overview.html */
	var HTML5Elements = new Array(

		/* The root element */
		'html',
		
		/* Document metadata */
		'head',
		'title',
		'base',
		'link',
		'meta',
		'style',
		
		/* Scripting */
		'script',
		'noscript',
		
		/* Sections */
		'body',
		'section',
		'nav',
		'article',
		'aside',
		'h1',
		'h2',
		'h3',
		'h4',
		'h5',
		'h6',
		'hgroup',
		'header',
		'footer',
		'address',
		
		/* Grouping content */
		'p',
		'hr',
		'br',
		'pre',
		'dialog',
		'blockquote',
		'ol',
		'ul',
		'li',
		'dl',
		'dt',
		'dd',
		
		/* Text-level semantics */
		'a',
		'q',
		'cite',
		'em',
		'strong',
		'small',
		'mark',
		'dfn',
		'abbr',
		'time',
		'progress',
		'meter',
		'code',
		'var',
		'samp',
		'kbd',
		'sub',
		'sup',
		'span',
		'i',
		'b',
		'bdo',
		'ruby',
		'rt',
		'rp',
		
		/* Edits */
		'ins',
		'del',
		
		/* Embedded content */
		'figure',
		'img',
		'iframe',
		'embed',
		'object',
		'param',
		'video',
		'audio',
		'source',
		'canvas',
		'map',
		'area',
		
		/* Tabular data */
		'table',
		'caption',
		'colgroup',
		'col',
		'tbody',
		'thead',
		'tfoot',
		'tr',
		'td',
		'th',
		
		/* Forms */
		'form',
		'fieldset',
		'label',
		'input',
		'button',
		'select',
		'datalist',
		'optgroup',
		'option',
		'textarea',
		'keygen',
		'output',
		
		/* Interactive elements */
		'details',
		'datagrid',
		'command',
		'bb',
		'menu',
		
		/* Miscellaneous elments */
		'legend',
		'div'

	);

	for(var a=0; a<HTML5Elements.length; a++) document.createElement(HTML5Elements[a]);

