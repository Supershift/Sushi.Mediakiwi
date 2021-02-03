using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Beta.GeneratedCms.Source.Library
{
    class Inline
    {
        internal static string Code(Console container)
        {
            return string.Format(@"
function ClassBehaviour(){
	/* properties */
		this.handlers			=	new Array();
	/* methods */
		// parse the document for classnames
		this.parseDocument		=	function(node){
										target = (node) ? node : document;
										// get all document nodes
										var allNodes = (target.all) ? target.all : target.getElementsByTagName(""*"");
										// for all tags
										for(var a=allNodes.length-1; a>=0; a--){ // for(var a=0; a<allNodes.length; a++){
											// if the item has a className
											if(allNodes[a].className){
												// get the classname
												nodeClass = allNodes[a].className;
												// for all behaviours
												for(var b=0; b<this.handlers.length; b++){
													// if the behaviour's name exists in the class name, apply it's events
													if(nodeClass.indexOf(this.handlers[b].name)>-1) this.handlers[b].start(allNodes[a]);
												}
											}
										}
									}
		// (cross)fader and pseudo event handler
		this.fader				=	new Fader;
		// helper functions
		this.utilities			=	new Utilities;
		// xml interface
		this.xmlDoc				=	new XmlDoc;
		// debug window
		this.console			=	new Console;
}
// create the main class-behaviour object
var classBehaviour = new ClassBehaviour;

// UTILITIES
	function Utilities(){
		this.screenHeight 		= 	function(){
										return (window.innerHeight) ? window.innerHeight : (document.documentElement.clientHeight) ? document.documentElement.clientHeight : document.body.clientHeight ;
									}
		// return a parameter from the url's query strings
		this.getQueryParameter 	= 	function(paramName, defaultValue){
										// split the query string at the parameter name
										var queryParameters = document.location.search.split(paramName+""="");
										// split the parameter value from the rest of the string
										var queryParameter = (queryParameters.length>1) ? queryParameters[1].split(""&"")[0] : null ;
										// return the value
										return (queryParameter!=null) ? queryParameter : defaultValue ;
									}
		// returns a string of parameters found in the classname which can be [eval]uated
		this.getClassParameter	=	function(targetNode, paramName, defaultValue){
										// get the class parameter from the classname
										var classParameter = targetNode.className;
										// split the classname between the parameter name
										classParameter = classParameter.split(paramName + '_');
										// split the second piece between spaces and take the first part,  if there are two pieces
										classParameter = (classParameter.length>1) ? classParameter[1].split(' ')[0] : null ;
										// return the value
										return (classParameter!=null) ? classParameter : defaultValue ;
									}
		// get the next node without worrying about text nodes
		this.nextNode			=	function(node, count){
										testNode = node;
										if(count==null) count = 1;
										// look for the next html node
										for(var a=0; a<count; a++){
											do {
												testNode = testNode.nextSibling;
												if(testNode==null) testNode = node;
											}while(testNode.nodeName.indexOf('#text')>-1);
										}
										// return it
										return testNode;
									}
		// get the previous node without worrying about text nodes
		this.previousNode		=	function(node, count){
										testNode = node;
										if(count==null) count = 1;
										// look for the previous html node
										for(var a=0; a<count; a++){
											do {
												testNode = testNode.previousSibling;
												if(testNode==null) testNode = node;
											}while(testNode.nodeName.indexOf('#text')>-1);
										}
										// return it
										return testNode;
									}
		// get the first real child node without worrying about text nodes
		this.firstNode			=	function(node, count){
										return this.nextNode(node.firstChild, count);
									}
		// get the value from a node
		this.getNodeValue	=	function(objNode){
									strValue = (objNode.childNodes.length>0) ? objNode.firstChild.nodeValue : null ;
									return (isNaN(strValue) || strValue==null) ? strValue : parseInt(strValue);
								}
		// set the value from a node
		this.setNodeValue	=	function(objNode,strValue){
									if(objNode.childNodes.length>0){
										objNode.firstChild.nodeValue = strValue;
									}else{
										var objNewNode = xmlDoc.createTextNode(strValue);
										objNode.appendChild(objNewNode);
									}
								}
		// get the number of a child
		this.getChildNumber =	function(objNode){
									var objNodes = objNode.parentNode.childNodes;
									var intTextNodes = 0;
									for(var intA=0; intA<objNodes.length; intA++){
										if(objNodes[intA].nodeName == '#text') intTextNodes += 1;
										if(objNode==objNodes[intA]) return intA - intTextNodes;
									}
									return null;
								}
		// returns the visible display state needed for this element
		this.getVisibleState	=	function(node){
										// what kind of node is this
										switch(node.nodeName.toLowerCase()){
											case 'table' : visibleState='table' ; break;
											case 'thead' : visibleState='table-header-group' ; break;
											case 'tfoot' : visibleState='table-footer-group' ; break;
											case 'tbody' : visibleState='table-row-group' ; break;
											case 'tr' : visibleState='table-row' ; break;
											case 'td' : visibleState='table-cell' ; break;
											case 'th' : visibleState='table-cell' ; break;
											default : visibleState='block';
										}
										// apply the state
										return (document.all && navigator.userAgent.indexOf('Opera')<0) ? 'block' : visibleState;
									}
	}
	
	function Fader(){
		/* properties */
		/* methods */
		this.getFade	=	function(node){
								var fadeValue = null;
								// get the fade value using the proper method
								if(typeof(node.style.MozOpacity)!='undefined')	fadeValue = Math.round(parseFloat(node.style.MozOpacity)*100);
								if(typeof(node.style.filter)!='undefined')		fadeValue = parseInt(node.filters.alpha.opacity);
								if(typeof(node.style.opacity)!='undefined')		fadeValue = Math.round(parseFloat(node.style.opacity)*100);
								// return the value
								return fadeValue;
							}
		this.setFade	=	function(node, amount){
								// set the fade value using the proper method
								if(typeof(node.style.MozOpacity)!='undefined')	node.style.MozOpacity = amount/100;
								if(typeof(node.style.filter)!='undefined')		node.style.filter = ""alpha(opacity="" + amount + "")"";
								if(typeof(node.style.opacity)!='undefined')		node.style.opacity = amount/100;
									/*
									filter:alpha(opacity=50);	imageobject.filters.alpha.opacity=opacity
									-moz-opacity: 0.5;			imageobject.style.MozOpacity=opacity/100
									opacity: 0.5;
									-khtml-opacity: 0.5;
									*/
							}
		this.fadeIn		=	function(idIn, step, delay, evalEvent){
								var cf = classBehaviour.fader;
								// get the fading object
								node = document.getElementById(idIn);
								// get the current fade
								fade = cf.getFade(node) + step;
								// if not 100%
								if((fade)<100){
									// set the new fade
									cf.setFade(node, fade);
									// next step
									cf.timeOut = setTimeout(""classBehaviour.fader.fadeIn('""+idIn+""',""+step+"",""+delay+"",'""+evalEvent+""')"", delay);
								// else
								}else{
									// set the fade to 100%
									cf.setFade(node, 100);
									// trigger the end event
									eval(evalEvent);
								}
							}
		this.fadeOut	=	function(idOut, step, delay, evalEvent){
								var cf = classBehaviour.fader;
								// get the fading object
								node = document.getElementById(idOut);
								// get the current fade
								fade = cf.getFade(node) - step;
								// if not 100%
								if(fade>0){
									// set the new fade
									cf.setFade(node, fade);
									// next step
									cf.timeOut = setTimeout(""classBehaviour.fader.fadeOut('""+idOut+""',""+step+"",""+delay+"",'""+evalEvent+""')"", delay);
								// else
								}else{
									// set the fade to 100%
									cf.setFade(node, 0);
									// trigger the end event
									eval(evalEvent);
								}
							}
		this.crossFade	=	function(idIn, idOut, amount, step, delay, evalEvent){
								var cf = classBehaviour.fader;
								// if the amount is not the end value yet
								if(amount<=100){
									// set the fade amounts
									if(idIn && idIn!=""""){
										// unhide the new page
										document.getElementById(idIn).style.display = 'block';
										// set the fade amount
										cf.setFade(document.getElementById(idIn), amount);
									}
									if(idOut && idOut!=""""){
										// unhide the new page
										document.getElementById(idOut).style.display = 'block';
										// set the fade amount
										cf.setFade(document.getElementById(idOut), 100-amount);
									}
									// construct the fade function
									var evalLoop = ""classBehaviour.fader.crossFade('""+idIn+""', '""+idOut+""', ""+(amount+step)+"", ""+step+"", ""+delay+"", '""+evalEvent+""')"";
									// repeat the fade
									setTimeout(evalLoop, delay);
								}else{
								// else
									// cancel the opacity style
									if(idIn && idIn!=""""){
										var node = document.getElementById(idIn);
										if(typeof(node.style.MozOpacity)!='undefined')	node.style.MozOpacity = 'auto';
										if(typeof(node.style.filter)!='undefined')		node.style.filter = 'none';
										if(typeof(node.style.opacity)!='undefined')		node.style.opacity = 'auto';
									}
									// hide the old page
									if(idOut && idOut!="""") document.getElementById(idOut).style.display = 'none';
									// trigger the end event
									cf.onEnd(evalEvent);
								}
							}
		this.grow			=	function(id, width, height, step, delay, evalEvent){
									var cf = classBehaviour.fader;
									// validate the new size
									if(!width) width = -1;
									if(!height) height = -1;
									// get the object
									resizeObject = document.getElementById(id);
									// get the object's size
									objectWidth = parseInt(resizeObject.offsetWidth);
									objectHeight = parseInt(resizeObject.offsetHeight);
									// compare the sizes
									tooNarrow = objectWidth<width;
									tooShort = objectHeight<height;
									// if the height isn't big enough
									if(tooNarrow || tooShort){
										// increase it one increment
										if(tooNarrow) resizeObject.style.width = (objectWidth + step) + 'px';
										if(tooShort) resizeObject.style.height = (objectHeight + step) + 'px';
										// repeat
										setTimeout(""classBehaviour.fader.grow('""+id+""', ""+width+"", ""+height+"", ""+step+"", ""+delay+"", '""+evalEvent+""');"", delay);
									// else
									}else{
										// trigger the end event
										cf.onEnd(evalEvent);
									}
								}
		this.shrink		=	function(id, width, height, step, delay, evalEvent){
									var cf = classBehaviour.fader;
									// validate the new size
									if(!width) width = 99999;
									if(!height) height = 99999;
									// get the object
									resizeObject = document.getElementById(id);
									// get the object's size
									objectWidth = parseInt(resizeObject.offsetWidth);
									objectHeight = parseInt(resizeObject.offsetHeight);
									// compare the sizes
									tooWide = objectWidth>width;
									tooTall = objectHeight>height;
									// if the height isn't big enough
									if(tooWide || tooTall){
										// increase it one increment
										if(tooWide) resizeObject.style.width = (objectWidth - step) + 'px';
										if(tooTall) resizeObject.style.height = (objectHeight - step) + 'px';
										// repeat
										setTimeout(""classBehaviour.fader.shrink('""+id+""', ""+width+"", ""+height+"", ""+step+"", ""+delay+"", '""+evalEvent+""');"", delay);
									// else
									}else{
										// trigger the end event
										cf.onEnd(evalEvent);
									}
								}
		/* events */
		this.onEnd		=	function(evalEvent){
								eval(evalEvent);
							}
	}
	
	function XmlDoc(){
		// properties
		this.queue			=	new Array();
		// methods
		this.addRequest	=	function(url, loadHandler, progressHandler, post, eventObject){
									// get the first free slot in the que
									index = this.queue.length;
									// add new request to the end of the que
									this.queue[index] = new HttpRequest();
									// set request constants
									this.queue[index].idx			=	index;
									this.queue[index].url			=	url;
									this.queue[index].post			=	post;
									this.queue[index].method		=	(post) ? 'POST' : 'GET' ;
									// request events
									this.queue[index].doOnLoad		=	loadHandler;
									this.queue[index].doOnProgress	=	progressHandler;
									this.queue[index].referObject	=	eventObject;
									// ask the queue handler to handle the next queued item
									this.handleQueue();
								}
		this.makeRequest	=	function(queued){
									// branch for native XMLHttpRequest object
									if(window.XMLHttpRequest){
										queued.request = new XMLHttpRequest();
										queued.request.onreadystatechange = this.progress;
										queued.request.open(queued.method, queued.url, true);
										queued.request.send(queued.post);
									// branch for IE/Windows ActiveX version
									}else if(window.ActiveXObject){
										queued.request = new ActiveXObject(""Microsoft.XMLHTTP"");
										queued.request.onreadystatechange = this.progress;
										queued.request.open(queued.method, queued.url, true);
										queued.request.send(queued.post);
									// if all else fails: load the document in an iFrame
									}else if(window.frames){
										// create an iframe to read the document in
 										objIframe = document.createElement(""IFRAME"");
 										objIframe.src = queued.url;
 										objIframe.id = ""feedimport0"";
 										objIframe.name = ""feedimport0"";
										objIframe.style = ""visibility : invisible;position : absolute;left : -1600px; top : -1600px;"";
											//objIframe.onload = xmlDoc_load; // Doesn't work in Opera
										// append the iframe to the document
 										document.body.appendChild(objIframe);
										// wait for the iframe to load
										this.wait();
									}
								}
		this.handleQueue	=	function(){
									queue = classBehaviour.xmlDoc.queue;
									// if the first item in the queue is a completed request
									if(queue.length>0){
										if(queue[0].ready==4 /*&& xmlDoc.queue[0].status==200*/){
											// remove the completed request
											queue.reverse();
											queue.length = queue.length - 1;
											queue.reverse();
										}
									}
									// if the first item in the queue isn't allready in progress
									if(queue.length>0){
										if(!(queue[0].ready<4 && queue[0].ready!=null)){
											this.makeRequest(queue[0]);
										}
									}
								}
		this.serialize		=	function serialize(xmlDoc){
									ser = new XMLSerializer();
									str = ser.serializeToString(xmlDoc);
									return(str);
								}
		// events
		this.progress		=	function(){
									queued = classBehaviour.xmlDoc.queue[0];
									// remember the readyState
									queued.ready = queued.request.readyState;
									// only if req shows ""complete""
									if(queued.request.readyState == 4){
										// remember the status
										queued.status = queued.request.status;
										// only if ""OK""
										if(queued.request.status == 200 || queued.request.status == 304){
											// update optional progress indicator code
											if(queued.doOnProgress) queued.doOnProgress(1, queued.referObject);
											// prepare the document
											queued.document = queued.request.responseXML;
											queued.text = queued.request.responseText;
											// trigger the load event
											if(queued.doOnLoad) queued.doOnLoad(queued.document, queued.referObject, queued.text);
											// request the next item in the queue to be handled
											classBehaviour.xmlDoc.handleQueue();
										}else{
											// update optional progress indicator code
											if(queued.doOnProgress) queued.doOnProgress(-1, queued.referObject);
										}
									}else{
										// update optional progress indicator code
										if(queued.doOnProgress) queued.doOnProgress(queued.request.readyState/4, queued.referObject);
									}
									// return the status if desired
									return queued.request.readyState;
								}
		this.wait			=	function(){
									queued = classBehaviour.xmlDoc.queue[0];
									// if the xml document has loaded in the iframe
									if(window.frames[""feedimport0""]){
										// define the xml document object
										queued.document = window.frames[""feedimport0""].document;
										queued.text = window.frames[""feedimport0""].document.body.innerHTML;
										// what to do after the xml document loads
										queued.doOnLoad(queued.document, queued.referObject, queued.text);
									// else try again in a while
									}else{
										setTimeout(""xmlDoc.wait()"",256);
									}
								}
	}
		function HttpRequest(){
			// request constants
			this.idx			=	null;
			this.url			=	null;
			this.post			=	null;
			this.method			=	'GET';
			// request events
			this.doOnLoad		=	null;
			this.doOnProgress	=	null;
			this.referObject	=	null;
			// request properties
			this.request		=	null;
			this.document		=	null;
			this.text			=	null;
			this.ready			=	null;
			this.status			=	null;
		}

	function Console(){
		// properties
		this.debugLog = new Array();
		this.dbg = null;
		this.tellerX = 0;
		this.interval = 0;
		this.maxLength = 128;
		this.strArr2Str= '';
		// methods
		this.debug 			= 	function(){
									// open the debug window if it isn't open yet
									if(this.dbg==null) this.dbg = window.open('','debugwindow','width=256,height=580,resizable=yes,scrollbars=yes');
									// add line count
									this.tellerX = this.tellerX + 1;
									// for all debugable arguments
									intDebugLine = this.debugLog.length;
									this.debugLog[intDebugLine] = '';
									for(var intA=0; intA<arguments.length; intA++){
										// recursive array splitter
										if(typeof(arguments[intA])=='object'){
											this. strArr2Str = '------Array-------<br />';
											this.plotArray(arguments[intA],0);
											this. strArr2Str += '---end-Array------<br />';
											arguments[intA] = this. strArr2Str;
										}
										// add a debug line
										this.debugLog[intDebugLine] += arguments[intA] + '&nbsp;';
									}
									// blit all debug lines
									if(this.tellerX>this.interval){
										this.dbg.document.open();
										this.dbg.document.write('<html><head><title>debug info</title></head><body>');
										tellerY = this.debugLog.length - 1;
										if(this.debugLog.length>this.maxLength){maxLines=this.maxLength}else{maxLines=this.debugLog.length}
										for(tellerZ=0 ; tellerZ < maxLines ; tellerZ++){
											this.dbg.document.writeln(this.debugLog[tellerY]+""<br />"");
											tellerY = tellerY-1;
										}
										this.dbg.document.write('</body></html>');
										this.dbg.document.close();
										this.tellerX = 0;
									}
								}
		this.plotArray 		= 	function(arrIn,intRecursion){
									// indentation
									var strSpacing = ''
									for(var intC=0; intC<intRecursion; intC++){
										strSpacing += '&nbsp;&nbsp;&nbsp;';
									}
									// plotting individual elements
									if(arrIn!=null){
										for(var intB=0; intB<arrIn.length; intB++){
											if(typeof(arrIn[intB])=='object'){
												this.plotArray(arrIn[intB],intRecursion+1)
											}else{
												this. strArr2Str += strSpacing + arrIn[intB] + '<br />'
											}
											// end of line
											if(intB==arrIn.length-1 && intRecursion==1){
												this. strArr2Str += '------------------<br />';
											}
										}
									}else{
										this. strArr2Str += 'NULL<br />';
									}
								}
		this.debugClear 	= 	function(){
									this.debugLog.length = 0;
								}
		this.debugHtml 		= 	function(string){
									string = '<xmp>' + string + '</xmp>'
									this.debug(string)
								}
	}
	
	/* legacy wrapper functions */
	function debug(){
		classBehaviour.console.debug(arguments);
	}
	/* /legacy wrapper functions */
	
// BEHAVIOURS
// replace in class
	// define this class behaviour
	function ClassMouseHover(){
		/* properties */
		this.name 		= 	'classMouseHover';
		/* methods */
		this.start		=	function(node){
								node.onmouseover = this.addHover;
								node.onmouseout = this.remHover;
							}
		this.hasNoStateClass 	= 	function(objNode){
										return (objNode.className.indexOf('link')<0 && objNode.className.indexOf('hover')<0 && objNode.className.indexOf('active')<0);
									}
		/* events */
		this.addHover 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var cmh = classBehaviour.classMouseHover;
								// replace link by hover
								objNode.className = (cmh.hasNoStateClass(objNode)) ? 'hover ' + objNode.className : objNode.className.replace('link','hover') ;
							}
		this.remHover 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var cmh = classBehaviour.classMouseHover;
								// replace hover by link
								objNode.className = (cmh.hasNoStateClass(objNode)) ? 'link ' + objNode.className : objNode.className.replace('hover','link') ;
							}
		this.addActive 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var cmh = classBehaviour.classMouseHover;
								// replace link by active
								objNode.className = objNode.className.replace('link','active') ;
								// replace hover by active
								objNode.className = objNode.className.replace('hover','active') ;
								// if there's still no active class
								if(cmh.hasNoStateClass(objNode)) objNode.className = 'active ' + objNode.className;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.classMouseHover = new ClassMouseHover;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.classMouseHover;
	
// make all sub elements fake the :hover attribute with .hover
	// define this class behaviour
	function PseudoHover(){
		/* properties */
		this.name 		= 	'pseudoHover';
		/* methods */
		this.start		=	function(node){
								// get all elements
								allNodes = node.getElementsByTagName('LI');
								// for all elements
								for(var a=0; a<allNodes.length; a++){
									// add the hover event
									classBehaviour.classMouseHover.start(allNodes[a]);
								}
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.pseudoHover = new PseudoHover;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.pseudoHover;
	
// replace in src sub-string
	// define this class behaviour
	function SrcMouseHover(){
		/* properties */
		this.name 			= 	'srcMouseHover';
		this.cache 			= new Array();
		/* methods */
		this.start			=	function(node){
									this.cacheImages(node);
									node.onmouseover = this.addHover;
									node.onmouseout = this.remHover;
								}
		this.cacheImages	 = 	function(that) {
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									// if this is not the image, it must be the parent
									if(!objNode.src) objNode = objNode.getElementsByTagName('IMG')[0];
									// replace link by hover
									var cacheIdx = this.cache.length;
									// hover version
									this.cache[cacheIdx] = new Image();
									this.cache[cacheIdx].src = objNode.src.replace('_link','_hover');
									// active version
									this.cache[cacheIdx+1] = new Image();
									this.cache[cacheIdx+1].src = objNode.src.replace('_link','_active');
								}
		/* events */
		this.addActive 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// if this is not the image, it must be the parent
								if(objNode.nodeName!='IMG') objNode = objNode.getElementsByTagName('IMG')[0];
								// replace link by active
								objNode.src = objNode.src.replace('_link','_active');
								// replace hover by active
								objNode.src = objNode.src.replace('_hover','_active');
							}
		this.addHover 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// if this is not the image, it must be the parent
								if(!objNode.src) objNode = objNode.getElementsByTagName('IMG')[0];
								// replace link by hover
								objNode.src = objNode.src.replace('_link','_hover');
							}
		this.remHover 	= 	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// if this is not the image, it must be the parent
								if(!objNode.src) objNode = objNode.getElementsByTagName('IMG')[0];
								// replace link by hover
								objNode.src = objNode.src.replace('_hover','_link');
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.srcMouseHover = new SrcMouseHover;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.srcMouseHover;
	
// hides nodes
	// hide the ""hideThisNode"" class behaviour by default 
	document.writeln(""<style>.hideThisNode{overflow:hidden; visibility:hidden; height:1px;}</style>"");
	// define this class behaviour
	function HideThisNode(){
		/* properties */
		this.name 		= 	'hideThisNode';
		/* methods */
		this.start		=	function(node){
								node.style.overflow = 'hidden';
								node.style.visibility = 'hidden';
								node.style.height = '1px';
								node.className = node.className.replace('showThisNode', 'hideThisNode');
								return true;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.hideThisNode = new HideThisNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.hideThisNode;
	
// explicit opposite of a hidden node
	// define this class behaviour
	document.writeln(""<style>.ShowThisNode{overflow:visible; visibility:visible; height:auto;}</style>"");
	function ShowThisNode(){
		/* properties */
		this.name 		= 	'showThisNode';
		/* methods */
		this.start		=	function(node){
								node.style.overflow = 'visible';
								node.style.visibility = 'visible';
								node.style.height = 'auto';
								node.className = node.className.replace('hideThisNode', 'showThisNode');
								return true;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.showThisNode = new ShowThisNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.showThisNode;
	
// show or hide a node
	// define this class behaviour
	document.writeln(""<style>.toggleNextNode{cursor:pointer;}</style>"");
	function ToggleNextNode(){
		/* properties */
		this.name 			= 	'toggleNextNode';
		this.nodes			=	new Array();
		this.step			=	50;
		this.delay			=	10;
		this.index			=	0;
		/* methods */
		this.start			=	function(node){
									// set the event handlers for the source node
									node.onclick = this.toggle;
									// add this node to the node list
									this.nodes[this.nodes.length] = node;
									// give this node an id if it doesn't have one yet
									node.id = (node.id) ? node.id : this.name + this.nodes.length ;
									// is this node marked as ""auto"", order it to open on a timeout
									autoDelay = classBehaviour.utilities.getClassParameter(node, 'auto', null);
									if(autoDelay){
										this.index++
										node.id = (node.id) ? node.id : this.name + this.index ;
										setTimeout('classBehaviour.toggleNextNode.toggle(document.getElementById(""'+node.id+'""))', autoDelay);
									}
								}
		this.shrink			=	function(label, container, finished){
									var t2n = classBehaviour.toggleNextNode;
									// deactivate the label
									label.className = label.className.replace('active','link');
									label.parentNode.className = label.parentNode.className.replace('active','link');
									// if the container isn't shrunk completely
									if(container.offsetHeight>t2n.step){
										// height before
										heightBefore = container.style.height;
										// reduce its height by an amount
										container.style.overflow = 'hidden';
										container.style.visibility = 'visible';
										container.style.height = (container.offsetHeight - t2n.step) + 'px';
										// height after
										heightAfter = container.style.height;
										// the resizing isnt' working, skip straight to the end situation
										finished = (heightBefore==heightAfter) ? classBehaviour.hideThisNode.start(container) : false ;
									}else{
										// close the container
										classBehaviour.hideThisNode.start(container);
									}
									// return the state
									return finished;
								}
		this.grow			=	function(label, container, finished){
									var t2n = classBehaviour.toggleNextNode;
									// activate the label
									label.className = label.className.replace('link','active').replace('hover','active');
									label.parentNode.className = label.parentNode.className.replace('link','active').replace('hover','active');
									// measure the height of all the childnodes of the container
									totalHeight = 0;
									contents = container.childNodes;
									for(var a=0; a<contents.length; a++){
										totalHeight += (contents[a].offsetHeight) ? contents[a].offsetHeight : 0 ;
									}
									// if the container isn't grown completely
									if(container.offsetHeight<totalHeight-t2n.step){
										// height before
										heightBefore = container.style.height;
										// increase its height by an amount
										container.style.overflow = 'hidden';
										container.style.visibility = 'visible';
										container.style.height = (container.offsetHeight + t2n.step) + 'px';
										// height after
										heightAfter = container.style.height;
										// the resizing isnt' working, skip straight to the end situation
										finished = (heightBefore==heightAfter) ? classBehaviour.showThisNode.start(container) : finished = false;
									}else{
										// open the container
										classBehaviour.showThisNode.start(container);
									}
									// return the state
									return finished;
								}
		this.findContainer	=	function(targetLabel, targetRecursion){
									// if there was a target recursion, recurse parent nodes
									if(targetRecursion) for(var a=0; a<parseInt(targetRecursion); a++) targetLabel = targetLabel.parentNode;
									// get the next sibling of the label, which should be the container
									targetObject = classBehaviour.utilities.nextNode(targetLabel);
									// pass it back
									return targetObject;
								}
		/* events */
		this.toggle 		= 	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									var t2n = classBehaviour.toggleNextNode;
									// get all information on this node
									targetLabel = objNode;
									targetContainerId = classBehaviour.utilities.getClassParameter(targetLabel, 'id', null);
								//	targetFamily = classBehaviour.utilities.getClassParameter(targetLabel, 'family', null);
									targetRecursion = classBehaviour.utilities.getClassParameter(targetLabel, 'parent', null);
									targetContainer = (targetContainerId) ? document.getElementById(targetContainerId) : t2n.findContainer(targetLabel, targetRecursion) ;
									targetGrows = (targetContainer.className.indexOf('hideThisNode')>-1);
									// call for the node to update
									t2n.update(targetLabel.id, targetGrows);
									// cancel the click
									return false;
								}
		this.update			=	function(id, grows){
									var t2n = classBehaviour.toggleNextNode;
									// finished marker
									finished = true;
									// get all information on this node
									targetLabel = document.getElementById(id);
									targetContainerId = classBehaviour.utilities.getClassParameter(targetLabel, 'id', null);
									targetFamily = classBehaviour.utilities.getClassParameter(targetLabel, 'family', null);
									targetRecursion = classBehaviour.utilities.getClassParameter(targetLabel, 'parent', null);
									targetContainer = (targetContainerId) ? document.getElementById(targetContainerId) : t2n.findContainer(targetLabel, targetRecursion) ;
									// for every node in the node-list
									for(var a=0; a<t2n.nodes.length; a++){
										// get its family
										peerLabel = t2n.nodes[a];
										peerContainerId = classBehaviour.utilities.getClassParameter(peerLabel, 'id', null);
										peerFamily = classBehaviour.utilities.getClassParameter(peerLabel, 'family', null);
										peerRecursion = classBehaviour.utilities.getClassParameter(peerLabel, 'parent', null);
										peerContainer = (peerContainerId) ? document.getElementById(peerContainerId) : t2n.findContainer(peerLabel, peerRecursion) ;
										// if this node belongs to the same family and is open but has a different id
										if(peerFamily==targetFamily && peerFamily!=null && peerContainer.className.indexOf('hideThisNode')<0 && peerContainer!=targetContainer){
											// close the container one step
											finished = this.shrink(peerLabel, peerContainer, finished);
										// else if this node has the same id and is open
										}else if(peerLabel.id==targetLabel.id && !grows){
											// close the container one step
											finished = this.shrink(targetLabel, targetContainer, finished);
										// else 
										}else if(peerLabel.id==targetLabel.id && grows){
											// open the container one step
											finished = this.grow(targetLabel, targetContainer, finished);
										}
									}
									// if any step marked the function as unfinished
									if(!finished){
										// repeat it
										setTimeout(""classBehaviour.toggleNextNode.update('""+id+""',""+grows+"")"", t2n.delay);
									}else{
										// notify the iframe resizer to update its size if needed
										if(document.body.className.indexOf('resizeIframe')>-1) classBehaviour.resizeIframe.delay();
									}
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.toggleNextNode = new ToggleNextNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.toggleNextNode;
	
// Open an overlay as a popup window
	// define this class behaviour
	function OpenLayerPopUp(){
		/* properties */
		this.name 		= 	'openLayerPopUp';
		this.step		=	10;
		this.begin		=	0;
		this.end		=	50;
		/* methods */
		this.start		=	function(node){
								// find the target layer
								targetPopUp = null;
								// process the node
								this.process(node);
								// the node's open button
								node.onclick = this.show;
								// open the popup immediately if required
								if(classBehaviour.utilities.getClassParameter(node, 'auto', 'no')=='yes') this.show(node);
							}
		this.fadeIn		=	function(id, amount){
								var olp = classBehaviour.openLayerPopUp;
								node = document.getElementById(id);
								nodes = node.getElementsByTagName('div');
								nodeShadow = nodes[0];
								nodeContent = nodes[1];
								// if the amount is not 50
								if(amount<olp.end){
									// hide the popup content
									nodeContent.style.visibility = 'hidden';
									// set the shadow's fade to the next step
									nodeShadow.style.display = 'block';
									if(typeof(nodeShadow.style.MozOpacity)!='undefined')	nodeShadow.style.MozOpacity = amount/100;
									if(typeof(nodeShadow.style.filter)!='undefined')		nodeShadow.style.filter = ""alpha(opacity="" + amount + "")"";
									if(typeof(nodeShadow.style.opacity)!='undefined')		nodeShadow.style.opacity = amount/100;
									// show the popup collection
									node.style.display = 'block';
									// repeat the fade
									setTimeout(""classBehaviour.openLayerPopUp.fadeIn('"" + id + ""',"" + (amount+olp.step) + "")"",10);
								}else{
									// show the popup content
									nodeContent.style.visibility = 'visible';
								}
							}
		this.fadeOut	=	function(id, amount){
								var olp = classBehaviour.openLayerPopUp;
								node = document.getElementById(id);
								nodes = node.getElementsByTagName('div');
								nodeShadow = nodes[0];
								nodeContent = nodes[1];
								// if the amount is not 100
								if(amount>olp.begin){
									// hide the popup content
									nodeContent.style.visibility = 'hidden';
									// set the fade to the next step
									if(typeof(nodeShadow.style.MozOpacity)!='undefined')	nodeShadow.style.MozOpacity = amount/100;
									if(typeof(nodeShadow.style.filter)!='undefined')		nodeShadow.style.filter = ""alpha(opacity="" + amount + "")"";
									if(typeof(nodeShadow.style.opacity)!='undefined')		nodeShadow.style.opacity = amount/100;
									// repeat the fade
									setTimeout(""classBehaviour.openLayerPopUp.fadeOut('"" + id + ""',"" + (amount-olp.step) + "")"",10);
								}else{
									// hide the popup content
									node.style.display = 'none';
									// hide the popup's shadow
									nodeShadow.style.display = 'none';
								}
							}
		/* events */
		this.process	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// prepare the popup's layout
							}
		this.show		=	function(that, id){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var olp = classBehaviour.openLayerPopUp;
								var submit = false;
								// adjust the fade parameters if it needs to open instantly
								olp.step = parseInt(classBehaviour.utilities.getClassParameter(objNode, 'step', 10));
								olp.begin = parseInt(classBehaviour.utilities.getClassParameter(objNode, 'begin', 0));
								olp.end = parseInt(classBehaviour.utilities.getClassParameter(objNode, 'end', 50));
								// is this a link with an href
								popUpId = (id) ? id : classBehaviour.utilities.getClassParameter(objNode, 'id', 'popup0') ;
								// get the popup object
								popUp = (popUpId) ? document.getElementById(popUpId) : objNode;
								// position the popup
									// the dimensions can not be measured with ""display:none;""
									popUp.style.visibility = 'hidden';
									popUp.style.display = 'block';
									// get the content segment
									popupContent = popUp.getElementsByTagName('DIV')[2];
									// how high is the content
									popupHeight = popupContent.offsetHeight;
									// how high is the screen
									screenHeight = popupContent.parentNode.offsetHeight;
									// center the popup
									popupContent.style.marginTop = Math.round((screenHeight-popupHeight)/2) + 'px' ;
									// the dimensions can not be measured with ""display:none;""
									popUp.style.visibility = 'visible';
									popUp.style.display = 'block';
								// if there is an iframe in the popup load the url into it
								popUpIframes = popUp.getElementsByTagName('iframe');
								popUpTitles = popUp.getElementsByTagName('span');
								if(popUpIframes.length>0){
									// is an href supplied
									if(objNode.href){
										// if the link had a title and if the popup has a title. put the link title in the popup
										if(objNode.title && popUpTitles.length>0) popUpTitles[0].innerHTML = objNode.title;
										// load the href in the iframe
										popUpIframes[0].src = objNode.href;
									// is this a form submit
									}else if(objNode.nodeName==""BUTTON"" || objNode.nodeName==""INPUT""){
										// find the form this button belongs to
										formNode = objNode.parentNode;
										while(formNode.nodeName!='FORM') formNode = formNode.parentNode;
										// change the form to submit to the iframe
										formNode.target = popUpIframes[0].id;
										// then change it back for the other sumbit buttons
										setTimeout(""document.getElementById('""+formNode.id+""').target='_self';"",512);
										// allow the function to submit
										submit = true;
									}
								}
								// find the close gadget
								popUpCloser = popUp.getElementsByTagName('a')[0];
								popUpCloser.onclick = olp.hide;
								// remove the scroll bars
								if(navigator.appVersion.indexOf('MSIE 6')>-1 || navigator.appVersion.indexOf('MSIE 5')>-1){
									// manage the scroll positions
									if(popUp.className.indexOf('fullHeightPopUp')<0){
										// reset the scroll position
										document.documentElement.scrollLeft = 0;
										document.documentElement.scrollTop = 0;
										// hide the scrollbars if needed
										document.body.parentNode.style.overflow = ""hidden"";
										// reposition the popup according to the scroll position
										popUp.getElementsByTagName('DIV')[2].style.marginTop = Math.round(document.documentElement.scrollTop + (screenHeight-popupHeight)/2) + 'px' ;
									}
									// size the shadow under the popup
									popUp.getElementsByTagName('DIV')[0].style.height = document.body.offsetHeight + 'px';
									// hide the selects
									allSelects = document.getElementsByTagName('select');
									for(var a=0; a<allSelects.length; a++) allSelects[a].style.visibility = 'hidden';
								}
								// fade the popup in
								olp.fadeIn(popUp.id, olp.begin);
								// mark the body with a class
								document.body.className += "" hasLayerPopUp"";
								// cancel the click
								return submit;
							}
		this.hide		=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var olp = classBehaviour.openLayerPopUp;
								// the popup object
								popUp = objNode.parentNode.parentNode.parentNode;
								// fade the popup out
								olp.fadeOut(popUp.id, 50);
								// restore the scroll bars
								if(navigator.appVersion.indexOf('MSIE 6')>-1 || navigator.appVersion.indexOf('MSIE 5')>-1){
									// show the scrollbars
									if(popUp.className.indexOf('fullHeightPopUp')<0) document.body.parentNode.style.overflow = ""auto"";
									// show the selects
									allSelects = document.getElementsByTagName('select');
									for(var a=0; a<allSelects.length; a++) allSelects[a].style.visibility = 'visible';
								}
								// unmark the body class
								document.body.className = document.body.className.replace("" hasLayerPopUp"", """");
								// clear the iframe contents
								popUpIframes = popUp.getElementsByTagName('iframe');
								if(popUpIframes.length>0){
									popUpIframes[0].src = """";
								}
								// cancel the click
								return false;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.openLayerPopUp = new OpenLayerPopUp;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.openLayerPopUp;
	
// Enrich a calendar control
	// define this class behaviour
	function DatePicker(){
		/* properties */
		this.name 			= 	'datePicker';
		this.monthNames 	= 	new Array('januari','februari','maart','april','mei','juni','juli','augustus','september','oktober','november','december');
		this.dateHtml		=	'<div class=""dateBorder"">\n'+
								'	<ul class=""controls"">\n'+
								'		<li><button class=""previous srcMouseHover""><img alt=""Previous Month"" src=""../images/icon_left_link.png""/></button></li>\n'+
								'		<li><button class=""next srcMouseHover""><img alt=""Next Month"" src=""../images/icon_right_link.png""/></button></li>\n'+
								'	</ul>\n'+
								'	<table class=""dateTable"">\n'+
								'		<caption>\n'+
								'			<select id=""month4"" name=""month4"" class=""month"">\n'+
								'				<option value=""0"">jan</option>\n'+
								'				<option value=""1"">feb</option>\n'+
								'				<option value=""2"">mar</option>\n'+
								'				<option value=""3"">apr</option>\n'+
								'				<option value=""4"">mei</option>\n'+
								'				<option value=""5"">jun</option>\n'+
								'				<option value=""6"">jul</option>\n'+
								'				<option value=""7"">aug</option>\n'+
								'				<option value=""8"">sep</option>\n'+
								'				<option value=""9"">okt</option>\n'+
								'				<option value=""10"">nov</option>\n'+
								'				<option value=""11"">dec</option>\n'+
								'			</select>\n'+
								'			<select id=""year4"" name=""year4"" class=""year"">\n'+
								'				<option value=""2007"" selected=""selected"">2007</option>\n'+
								'			</select>\n'+
								'		</caption>\n'+
								'		<thead>\n'+
								'			<tr>\n'+
								'				<th scope=""col"">s</th>\n'+
								'				<th scope=""col"">m</th>\n'+
								'				<th scope=""col"">t</th>\n'+
								'				<th scope=""col"">w</th>\n'+
								'				<th scope=""col"">t</th>\n'+
								'				<th scope=""col"">f</th>\n'+
								'				<th scope=""col"">s</th>\n'+
								'			</tr>\n'+
								'		</thead>\n'+
								'		<tbody>\n'+
								'			<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>\n'+
								'			<tr><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td>{13}</td></tr>\n'+
								'			<tr><td>{14}</td><td>{15}</td><td>{16}</td><td>{17}</td><td>{18}</td><td>{19}</td><td>{20}</td></tr>\n'+
								'			<tr><td>{21}</td><td>{22}</td><td>{23}</td><td>{24}</td><td>{25}</td><td>{26}</td><td>{27}</td></tr>\n'+
								'			<tr><td>{28}</td><td>{29}</td><td>{30}</td><td>{31}</td><td>{32}</td><td>{33}</td><td>{34}</td></tr>\n'+
								'			<tr><td>{35}</td><td>{36}</td><td>{37}</td><td>{38}</td><td>{39}</td><td>{40}</td><td>{41}</td></tr>\n'+
								'		</tbody>\n'+
								'	</table>\n'+
								'</div>';
		/* methods */
		this.start		=	function(node){
								classBehaviour.toggleNextNode.start(node);
								// construct the calendar html
								this.build(node);
								// set the event handlers for this node
								node.onclick = this.open;
							}
		this.set		=	function(calendarNode, date){
								dpr = classBehaviour.datePicker;
								// get the next and previous buttons
								calendarNode.getElementsByTagName('button')[0].onclick = dpr.previous;
								calendarNode.getElementsByTagName('button')[1].onclick = dpr.next;
								// set the month dropdown
								monthNode = calendarNode.getElementsByTagName('select')[0];
								monthNode.selectedIndex = date.getMonth();
								monthNode.onchange = dpr.update;
								// set the year dropdown
								yearSelect = calendarNode.getElementsByTagName('select')[1];
								yearOptions = yearSelect.getElementsByTagName('option');
								currentYear = date.getFullYear();
								if(yearOptions.length==1){
									yearOption = yearOptions[0].cloneNode(true);
									for(var a=currentYear-100; a<currentYear+10; a++ ){
										yearOption = yearSelect.getElementsByTagName('option')[0].cloneNode(true);
										yearOption.value = a;
										yearOption.selected = (a==currentYear) ? 'selected' : '' ;
										yearOption.text = a;
										yearSelect.appendChild(yearOption);
									}
									yearSelect.removeChild(yearSelect.getElementsByTagName('option')[0]);
									yearSelect.onchange = dpr.update;
								}else{
									for(var a=0; a<yearOptions.length; a++) yearOptions[a].selected = (yearOptions[a].value==currentYear) ? 'selected' : '' ;
								}
								// clear out the previous month
								daySlots = calendarNode.getElementsByTagName('td');
								for(var a=0; a<daySlots.length; a++){
									daySlots[a].className = ""empty"";
									daySlots[a].innerHTML = """";
									daySlots[a].onclick = null;
								}
								// fill the new month
								currentDay = new Date(date.getFullYear(), date.getMonth(), 1);
								nextDay = new Date(date.getFullYear(), date.getMonth(), 2);
								startWeekDay = currentDay.getDay()-1;
								while(currentDay.getDate() < nextDay.getDate()){
									// put the date on the weekday cell
									daySlots[currentDay.getDate()+startWeekDay].innerHTML = currentDay.getDate();
									daySlots[currentDay.getDate()+startWeekDay].className = """";
									daySlots[currentDay.getDate()+startWeekDay].onclick = dpr.pick;
									// next date
									currentDay = new Date(currentDay.getFullYear(), currentDay.getMonth(), currentDay.getDate()+1);
									nextDay = new Date(nextDay.getFullYear(), nextDay.getMonth(), nextDay.getDate()+1);
								}
							}
		this.build		=	function(node){
								calendar = classBehaviour.utilities.nextNode(node);
								calendar.innerHTML = this.dateHtml;
								classBehaviour.parseDocument(calendar);
							}
		/* events */
		this.open		=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var dpr = classBehaviour.datePicker;
								// OBJECTS
								calendar = classBehaviour.utilities.nextNode(objNode);
								// STARTING DATE
								// get the input from the field
								// turn it into a date
								// if the result is a date
									// use it as a start date
								// else
									// take the current date
									date = new Date();
								// CONSTRUCT THE CALENDAR AT THE GIVEN DATE
								dpr.set(calendar, date);
								// POSITION THE CALENDAR
								// get the position
								screenXpos = (typeof(event)!='undefined') ? (event.clientX) : (that.pageX);
								screenYpos = (typeof(event)!='undefined') ? (event.clientY) : (that.pageY);
								// if the position is too close to the edge
								calendarWidth = calendar.firstChild.offsetWidth;
								screenWidth = (window.innerWidth) ? window.innerWidth : (document.documentElement.clientWidth) ? document.documentElement.clientWidth : document.body.clientWidth ;
								if(screenXpos+calendarWidth > screenWidth+window.pageXOffset) screenXpos -= calendarWidth;
								// if the position is too close to the bottom
								calendarHeight = calendar.firstChild.offsetHeight;
								screenHeight = (window.innerHeight) ? window.innerHeight : (document.documentElement.clientHeight) ? document.documentElement.clientHeight : document.body.clientHeight ;
								if(screenYpos+calendarHeight+10 > screenHeight+window.pageYOffset) screenYpos -= calendarHeight;
								// set the position
								calendar.style.left = screenXpos + 'px';
								calendar.style.top = screenYpos + 'px';
								// SHOW THE CALENDAR
								classBehaviour.toggleNextNode.toggle(objNode);
							}
		this.update	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var dpr = classBehaviour.datePicker;
								// get both selectors from the parent node
								selectors = objNode.parentNode.getElementsByTagName('select');
								// get the month
								month = parseInt(selectors[0].value);
								// get the year
								year = parseInt(selectors[1].value);
								// make a date out of it
								date = new Date(year, month, 1);
								// update the calendar
								dpr.set(objNode.parentNode.parentNode.parentNode.parentNode,date);
							}
		this.next		=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var dpr = classBehaviour.datePicker;
								// get the calendar
								calendar = objNode.parentNode.parentNode.parentNode.parentNode;
								// get the displayed date
								month = parseInt(objNode.parentNode.parentNode.parentNode.getElementsByTagName('select')[0].value);
								year = parseInt(objNode.parentNode.parentNode.parentNode.getElementsByTagName('select')[1].value);
								// add a month
								date = new Date(year,month+1,1);
								// build the calendar
								dpr.set(calendar,date);
							}
		this.previous	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var dpr = classBehaviour.datePicker;
								// get the calendar
								calendar = objNode.parentNode.parentNode.parentNode.parentNode;
								// get the displayed date
								month = parseInt(objNode.parentNode.parentNode.parentNode.getElementsByTagName('select')[0].value);
								year = parseInt(objNode.parentNode.parentNode.parentNode.getElementsByTagName('select')[1].value);
								// subsctract a month
								date = new Date(year,month-1,1);
								// build the calendar
								dpr.set(calendar,date);
							}
		this.pick		=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// get the selected day
								dayValue = objNode.innerHTML;
								// get the selected month
								calendarTitle = objNode.parentNode.parentNode.parentNode.getElementsByTagName('caption')[0];
								month = calendarTitle.getElementsByTagName('select')[0];
									// monthValue = month[month.selectedIndex].innerHTML;
								monthValue = parseInt(month.value) + 1;
								// get the selected year
								year = calendarTitle.getElementsByTagName('select')[1];
								yearValue = year[year.selectedIndex].innerHTML;
								// put it in the input field
								targetFields = objNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.getElementsByTagName('input');
								targetFields[0].value = ((dayValue+'').length==1) ? '0' + dayValue  : dayValue;
								targetFields[1].value = ((monthValue+'').length==1) ? '0' + monthValue : monthValue;
								targetFields[2].value = yearValue;
								// close the calendar
								calendarButton = objNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.getElementsByTagName('button')[0];
								classBehaviour.toggleNextNode.toggle(calendarButton);
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.datePicker = new DatePicker;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.datePicker;
	
// Turn an element into an HTML-editor
	// define this class behaviour
	function EditableContents(){
		/* properties */
		this.name 			= 	'editableContents';
		this.containerType	=	'FIELDSET';
		this.buttonType		=	'BUTTON';
		this.stylesheet		=	'<link href=""../styles/markup.css"" type=""text/css"" rel=""StyleSheet"">';
		this.htmlToolBar	=	'<li><button class=""cmd_toggle srcMouseHover""><img alt=""Text"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_toggleHtml_link.png""/></button></li>\n'+
								'<li><button class=""cmd_bigger srcMouseHover""><img alt=""Resize"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_bigger_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_redo srcMouseHover""><img alt=""Redo"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_redo_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_undo srcMouseHover""><img alt=""Undo"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_undo_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_copy srcMouseHover""><img alt=""Copy"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_copy_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_cut srcMouseHover""><img alt=""Cut"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_cut_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_paste srcMouseHover""><img alt=""Paste"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_paste_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_delete srcMouseHover""><img alt=""Delete"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_delete_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_formatblock arg_h1 srcMouseHover""><img alt=""H1"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_h1_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_formatblock arg_h2 srcMouseHover""><img alt=""H2"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_h2_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_formatblock arg_h3 srcMouseHover""><img alt=""H3"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_h3_link.png""/></button></li>\n'+
								'<li class=""disabled""><button class=""cmd_formatblock arg_p srcMouseHover""><img alt=""P"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_p_link.png""/></button></li>\n'+
								'<li><button class=""cmd_bold srcMouseHover""><img alt=""Bold"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_bold_link.png""/></button></li>\n'+
								'<li><button class=""cmd_italic srcMouseHover""><img alt=""Italic"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_italic_link.png""/></button></li>\n'+
								'<li><button class=""cmd_underline srcMouseHover""><img alt=""Underline"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_underline_link.png""/></button></li>\n'+
								'<li><button class=""cmd_insertorderedlist srcMouseHover""><img alt=""Ordered List"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_insertorderedlist_link.png""/></button></li>\n'+
								'<li><button class=""cmd_insertunorderedlist srcMouseHover""><img alt=""Unordered List"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_insertunorderedlist_link.png""/></button></li>\n'+
								'<li><button class=""cmd_outdent srcMouseHover""><img alt=""Outdent"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_outdent_link.png""/></button></li>\n'+
								'<li><button class=""cmd_indent srcMouseHover""><img alt=""Indent"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim//images/cmd_indent_link.png""/></button></li>\n'+
								'<li>\n'+
								'	<button class=""toggleNextNode""><img alt=""Link"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_createlink_link.png""/></button>\n'+
								'</li>';
		this.textToolBar	=	'<li><button class=""cmd_toggle srcMouseHover""><img alt=""HTML"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_toggleText_link.png""/></button></li>\n'+
								'<li><button class=""cmd_bigger srcMouseHover""><img alt=""Resize"" src=""/Sushi.Mediakiwi.AppCentre/repository/wim/images/cmd_bigger_link.png""/></button></li>';
		this.index			=	0;
		this.interval		=	null;
		this.resizing		=	false;
		/* methods */
		this.start			=	function(node){
									// prepare the editor
									this.setCanvas(node);
									this.setControls(node);
									// sync loop between the textarea and the iframe
									clearInterval(this.interval);
									this.interval = setInterval(""classBehaviour.editableContents.sync()"", 256);
								}
		this.setCanvas		=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									// store the container type
									this.containerType = objNode.parentNode.nodeName;
									// get the textarea
									textArea = objNode;
									// hide the textarea
									textArea.style.display = 'none';
									// if there's no iframe, make one
									iframes = objNode.parentNode.getElementsByTagName('iframe');
									if(iframes.length==0){
										// add an iframe to the canvas
										newIframe = document.createElement('iframe');
										newIframe.id = this.name + this.index;
										newIframe.setAttribute(""frameBorder"", ""0"");
										newIframe.setAttribute(""scrolling"", ""auto"");
										objNode.parentNode.insertBefore(newIframe, objNode.nextSibling);
									}else{
										this.index++;
										newIframe = iframes[0];
										newIframe.id = this.name + this.index;
										newIframe.style.display = 'block';
									}
									// transform the iframe into an editor
									editor = this.getEditor(newIframe.id);
									editor.designMode = 'on';
									// set the initial content
									editor.open();
									editor.write(textArea.value + this.stylesheet);
									editor.close();
								}
		this.setControls	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									
									// get the button bars
									buttonBars = objNode.parentNode.getElementsByTagName('ul');

									// HTML BUTTON BAR
									// get the html editor button bar
									buttonBar = buttonBars[0];
									// fill it with the html toolbar prototype
									buttonBar.innerHTML = this.htmlToolBar;
									classBehaviour.parseDocument(buttonBar);
									// get the buttons
									buttons = buttonBar.getElementsByTagName(this.buttonType);
									// for each button in the button bar
									for(var a=0; a<buttons.length; a++)
										// set the event handlers for the edit buttons
										if(buttons[a].className.indexOf('cmd_')>-1) buttons[a].onmousedown = this.editCommand;
									// show the button bar
									buttonBar.style.display = 'block';
									
									// TEXT BUTTON BAR
									// get the text editor button bar
									buttonBar = buttonBars[1];
									// fill it with the html toolbar prototype
									buttonBar.innerHTML = this.textToolBar;
									classBehaviour.parseDocument(buttonBar);
									// get the buttons
									buttons = buttonBar.getElementsByTagName(this.buttonType);
									// for each button in the button bar
									for(var a=0; a<buttons.length; a++)
										// set the event handlers for the edit buttons
										if(buttons[a].className.indexOf('cmd_')>-1) buttons[a].onmousedown = this.editCommand;
									// hide the button bar
									buttonBar.style.display = 'none';
								}
		this.getEditor		=	function(id){
									return (document.getElementById(id).contentDocument) ? 
												document.getElementById(id).contentDocument : 
													document.frames[id].document ;
								}
		this.sync			=	function(){
									// for all textareas
									allTextareas = document.getElementsByTagName('textarea');
									for(var a=0; a<allTextareas.length; a++){
										// if it's part of this class
										if(allTextareas[a].className.indexOf('editableContents')>-1){
											// get all iframes that may be associated with the editor
											allIframes = allTextareas[a].parentNode.getElementsByTagName('iframe');
											if(allIframes.length>0){
												// update the textarea from the iframe editor
												editorId = allIframes[0].id;
												editor = classBehaviour.editableContents.getEditor(editorId);
												// if the iframe is replacing the textarea
												if(allTextareas[a].style.display=='none'){
													allTextareas[a].value = editor.documentElement.firstChild.nextSibling.innerHTML.split('<LINK')[0].split('<link')[0];
												}else{
													editor.documentElement.firstChild.nextSibling.innerHTML = allTextareas[a].value + classBehaviour.editableContents.stylesheet;
												}
											}
										}
									}
								}
		this.resize		=	function(container){
									if(!this.resizing){
										// get the editor
										iFrame = container.getElementsByTagName('iframe')[0];
										textArea = container.getElementsByTagName('textarea')[0];
										// what is the mode of the editor
										editor = (textArea.style.display=='none') ? iFrame : textArea;
										// if the editor wasn't marked big
										if(editor.className.indexOf('biggerEditor')<0){
											// mark is as big
											editor.className = 'biggerEditor ' + editor.className;
											// and lock this function
											classBehaviour.editableContents.resizing = true;
											// make the editor twice as big
												// editor.style.height = (editor.offsetHeight * 3) + 'px';
											classBehaviour.fader.grow(editor.id, null, editor.offsetHeight*3, 20, 10, 'classBehaviour.editableContents.resizing=false;');
										//else
										}else{
											// remove it mark
											editor.className = editor.className.replace('biggerEditor', '');
											// and lock this function
											classBehaviour.editableContents.resizing = true;
											// make the editor half as big
												//editor.style.height = Math.round(editor.offsetHeight / 3) + 'px';
											classBehaviour.fader.shrink(editor.id, null, editor.offsetHeight/3, 20, 10, 'classBehaviour.editableContents.resizing=false;');
										}
									}
								}
		/* events */
		this.toggle		=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									var ecs = classBehaviour.editableContents;
									// get the editor's container
									container = objNode.parentNode.parentNode.parentNode;
									// get the elements from this editor
									iframes = container.getElementsByTagName('iframe');
									buttonBars = container.getElementsByTagName('ul');
									textareas = container.getElementsByTagName('textarea');
									// if the textearea is invisible
									if(textareas[0].style.display=='none'){
										// hide the button bar
										buttonBars[0].style.display = 'none';
										buttonBars[1].style.display = 'block';
										// hide the iframe
										iframes[0].style.display = 'none';
										// show the textarea
										textareas[0].style.display = 'block';
									}else{
										// show the button bar
										buttonBars[0].style.display = 'block';
										buttonBars[1].style.display = 'none';
										// show the iframe
										iframes[0].style.display = 'block';
										// hide the textarea
										textareas[0].style.display = 'none';
									}
								}
		this.editCommand	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									var ecs = classBehaviour.editableContents;
									// get the editor canvas
									container = objNode;
									while(container.nodeName.indexOf(classBehaviour.editableContents.containerType)<0) container = container.parentNode;
									editor = ecs.getEditor(container.getElementsByTagName('iframe')[0].id) ;
									// get the selected section
									selection = 	(editor.selection) ? editor.selection.createRange().text : 
														(editor.getSelection) ? editor.getSelection() : 
															(editor.createRange) ? editor.createRange() : 
																null ;
									// gather the command parameters
									commandName = classBehaviour.utilities.getClassParameter(objNode, 'cmd', '');
									commandArgument = classBehaviour.utilities.getClassParameter(objNode, 'arg', '');
									// exceptions
									switch(commandName){
										case 'insertimage' : commandArgument = classBehaviour.utilities.previousNode(objNode).value; break;
										case 'createlink' : commandArgument = classBehaviour.utilities.previousNode(objNode).value; break;
										default : break;
									}
									if(commandName=='toggle') classBehaviour.editableContents.toggle(objNode);
									if(commandName=='bigger') classBehaviour.editableContents.resize(container);
									// execute the command
									if(selection) editor.execCommand(commandName, false, commandArgument);
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.editableContents = new EditableContents;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.editableContents;
	
// Insert a node imported from an xml file
	// define this class behaviour
	function InsertParentNode(){
		/* properties */
		this.name 		= 	'insertParentNode';
		/* methods */
		this.start		=	function(node){
								// set the event for this button
								node.onclick = this.importNode;
							}
		/* events */
		this.importNode	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// what xml doc needs to be imported
								xmlSourceId = classBehaviour.utilities.getClassParameter(objNode,'src','frmXmlSource');
								xmlUrl = document.getElementById(xmlSourceId).value;
								// place the http request
								classBehaviour.xmlDoc.addRequest(xmlUrl, classBehaviour.insertParentNode.inserNode, classBehaviour.insertParentNode.waiting, null, objNode);
								// cancel the click
								return false;
							}
		this.waiting	=	function(loadStatus){
								// debug(loadStatus);
							}
		this.inserNode =	function(importedXml, referedNode, importedText){
								// how deep is the button in the moving node
								nodeDepth = parseInt(classBehaviour.utilities.getClassParameter(referedNode,'parent','1'));
								for(var a=0; a<nodeDepth; a++) referedNode = referedNode.parentNode;
								// create a new node somewhere out of sight
								newNode = document.createElement('div');
								newNode.style.position = 'absolute';
								newNode.style.left = '-1000px';
								newNode.style.top = '-1000px';
								newNode.style.visibility = 'hidden';
								newNode.id = 'insertParentNode_f00';
								document.body.appendChild(newNode);
								temporaryNode = document.getElementById('insertParentNode_f00');
								// insert the imported html
								temporaryNode.innerHTML = importedText.replace('<?xml version=""1.0""?>','').replace('<root>','').replace('</root>','');
								importedNode = temporaryNode.removeChild(classBehaviour.utilities.firstNode(temporaryNode));
								// get the contents and insert them near the refered node
								referedNode.parentNode.insertBefore(importedNode, referedNode.nextSibling);
								insertedNode = classBehaviour.utilities.nextNode(referedNode);
								// destroy the temporary node
								removedTempNode = document.body.removeChild(temporaryNode);
								// (maybe) execute the clasbehaviours again
								classBehaviour.parseDocument(insertedNode);
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.insertParentNode = new InsertParentNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.insertParentNode;	
	
// Clone the previous node to after this one
	// define this class behaviour
	function CloneParentNode(){
		/* properties */
		this.name 		= 	'cloneParentNode';
		/* methods */
		this.start		=	function(node){
								// set the event for this button
								node.onclick = this.clone;
							}
		/* events */
		this.clone	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// how deep is the button in the moving node
								nodeDepth = parseInt(classBehaviour.utilities.getClassParameter(objNode,'parent','1'));
								for(var a=0; a<nodeDepth; a++) objNode = objNode.parentNode;
								// clone the contents
								newNode = objNode.cloneNode(true);
								// add the cached contents before this button
								objNode.parentNode.insertBefore(newNode, objNode);
								// (maybe) execute the clasbehaviours again
								classBehaviour.parseDocument(classBehaviour.utilities.previousNode(objNode));
								// cancel the click
								return false;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.cloneParentNode = new CloneParentNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.cloneParentNode;	
	
// Change the order of page elements
	// define this class behaviour
	function MoveParentNode(){
		/* properties */
		this.name 		= 	'moveParentNode';
		this.instances	=	new Array();
		/* methods */
		this.start		=	function(node){
								// set the event for this button
								node.onclick = this.move;
								// store the button for later reference
								this.instances[this.instances.length] = node;
								// test if the node can move up or down
								this.checkLimit(node);
							}
		this.checkLimit	=	function(node){
								// if the previous node is ""fixed"" disable the button
								rootNode = node.parentNode.parentNode.parentNode;
								testNode = (node.className.indexOf('down')>-1) ? classBehaviour.utilities.nextNode(rootNode) : classBehaviour.utilities.previousNode(rootNode);
								if(testNode.className.indexOf('fixed')>-1 || rootNode.className.indexOf('fixed')>-1){
									node.parentNode.className += (node.parentNode.className.indexOf('disabled')<0) ? ' disabled' : '' ;
								}else{
									node.parentNode.className = node.parentNode.className.replace(' disabled', '');
								}
							}
		/* events */
		this.move		=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									var mpn = classBehaviour.moveParentNode;
									// up or down?
									goingDown = (classBehaviour.utilities.getClassParameter(objNode,'move','down')=='down');
									// is the button disabled?
									if(objNode.parentNode.className.indexOf('disabled')<0){
										// how deep is the button in the moving node
										nodeDepth = parseInt(classBehaviour.utilities.getClassParameter(objNode,'parent','1'));
										for(var a=0; a<nodeDepth; a++) objNode = objNode.parentNode;
										// the node before which this one is to be inserted
										targetNode = 	(goingDown) ? 
															classBehaviour.utilities.nextNode(classBehaviour.utilities.nextNode(objNode)) : 
																classBehaviour.utilities.previousNode(objNode);
										if(targetNode!=objNode && targetNode.className.indexOf('fixed')<0){
											// what is the container node
											containerNode = objNode.parentNode;
											// remove the node
											cachedNode = containerNode.removeChild(objNode);
											// insert the node in it's new location
											containerNode.insertBefore(cachedNode, targetNode);
											// re-init the classbehaviours within
											classBehaviour.parseDocument(classBehaviour.utilities.previousNode(targetNode));
										}
									}
									// recheck the button status
									for(var a in mpn.instances) mpn.checkLimit(mpn.instances[a]);
									// cancel the click
									return false;
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.moveParentNode = new MoveParentNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.moveParentNode;

// Clear a node from the document
	// define this class behaviour
	function DeleteParentNode(){
		/* properties */
		this.name 		= 	'deleteParentNode';
		/* methods */
		this.start		=	function(node){
								// set the event for this button
								node.onclick = this.remove;
							}
		/* events */
		this.remove	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// how deep is the button in the moving node
								nodeDepth = parseInt(classBehaviour.utilities.getClassParameter(objNode,'parent','1'));
								for(var a=0; a<nodeDepth; a++) objNode = objNode.parentNode;
								// what is the container node
								containerNode = objNode.parentNode;
								// remove the parent node
								cachedNode = containerNode.removeChild(objNode);
								// cancel the click
								return false;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.deleteParentNode = new DeleteParentNode;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.deleteParentNode;

// change the focus to a table row
	// define this class behaviour
	function FocusRow(){
		/* properties */
		this.name 		= 	'focusRow';
		this.previous	=	null;
		/* methods */
		this.start		=	function(node){
								node.onclick = this.highlight;
								// add events to all cells
								for(var a=0; a<node.childNodes.length; a++) if(node.childNodes[a].nodeName!='#text') node.childNodes[a].onclick = this.highlight;
								// add events to all form elements
								inputs = node.getElementsByTagName('*');
								for(var a=0; a<inputs.length; a++) if(inputs[a].nodeName!='#text') inputs[a].onfocus = this.highlight;
								// add events to all iframes
								iframes = node.getElementsByTagName('iframe');
								for(var a=0; a<iframes.length; a++) iframes[a].onclick = this.highlight;
							}
		/* events */
		this.highlight	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var frw = classBehaviour.focusRow;
								// find the row
								parentRow = objNode;
								while(parentRow.nodeName!='TR') parentRow = parentRow.parentNode;
								// deactivate the previous row
								if(frw.previous) if(parentRow!=frw.previous) frw.previous.className = frw.previous.className.replace('active','link');
								// add the active class
								parentRow.className = parentRow.className.replace('link','active');
								// store the current row
								frw.previous = parentRow;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.focusRow = new FocusRow;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.focusRow;
	
// Construct an empty stylesheet based on the hierarchy of tags
	// define this class behaviour
	function MakeStylesheet(){
		/* properties */
		this.name 				=	'makeStylesheet';
		this.styleSheet			= 	""/* "" + document.location.href.split('/')[document.location.href.split('/').length-1] + "" */\n"";
		this.referenceCss		=	"""";
		this.rootObject			= 	document.body;
		/* methods */
		this.start				=	function(node){
										// make a reference stylesheet from the current stylesheets
										this.makeReferenceCss();
										// create a button to publish the stylesheet
										document.writeln('<button onclick=""classBehaviour.makeStylesheet.showNodeClasses()"" style=""position : absolute; left : 0px; top : 0px; width : 15em;"">Click to create stylesheet.</button>');
										/*
										debug(
											document.styleSheets[0].cssRules[1].selectorText,
											document.styleSheets[0].cssRules[1].cssText,
											document.styleSheets[0].cssRules[1].style.getPropertyValue('font-family')
										);
										*/
									}
		this.makeReferenceCss	=	function(){
										for(var a=0; a<document.styleSheets.length; a++)
											if(document.styleSheets[a].cssRules)
												for(var b=0; b<document.styleSheets[a].cssRules.length; b++)
													this.referenceCss += document.styleSheets[a].cssRules[b].selectorText + ' {}\n\t';
									}
		this.isFormElement		=	function(node){
										return (('INPUT,SELECT,TEXTAREA,BUTTON').indexOf(node.nodeName)>-1);
									}
		this.isClassBehaviour	=	function(newEntry){
										foundHandler = false;
										// for all behaviours, if the behaviour's name exists in the class name, apply it's events
										for(var b=0; b<classBehaviour.handlers.length; b++)
											foundHandler = (newEntry.indexOf(classBehaviour.handlers[b].name)>-1) ? true : foundHandler;
										// report back
										return foundHandler;
									}
		this.isInStylesheet		=	function(newEntry){
										foundStyle = false;
										// clean the new entry
										newEntry = newEntry.replace(/\t/gi,'').replace(' {}\n','').replace(',','');
										// if the style allready exists in this constructed stylesheet
										foundStyle = (this.styleSheet.indexOf(newEntry)>-1);
										// if the style allready exists in any rule in another stylesheet
										foundStyle = (this.referenceCss.indexOf(newEntry)>-1) ? true : foundStyle ;
										// report back
										return foundStyle;
									}
		this.getNodeClasses		=	function(objNode, intRecursion, prefix){
										var strTabs = '';
										var idPrefix, classPrefix, tagPrefix, addPrefix;
										var newEntry = '';
										// for every recursion add one tab
										for(var intB=0; intB<intRecursion; intB++) strTabs += '\t';
										// get the child nodes
										var objChildNodes = objNode.childNodes;
										// for every childnode
										for(var intA=0; intA<objChildNodes.length; intA++){
											// reset prefixes
											idPrefix = '';
											classPrefix = '';
											tagPrefix = '';
											addPrefix = '';
											// if it has an id, but is not a form element
											if(typeof(objChildNodes[intA].id)!='undefined' && !this.isFormElement(objChildNodes[intA])){
												if(objChildNodes[intA].id!=''){
													// add class to stylesheet prototype
													newEntry = strTabs + '#' + objChildNodes[intA].id + ' {}\n';
														// strStyleSheet += strTabs + prefix + '#' + objChildNodes[intA].id + ' {}\n'
													// add this style only if there's not double
													if(!this.isInStylesheet(newEntry)) this.styleSheet += newEntry;
													// update the prefix
													idPrefix = '#' + objChildNodes[intA].id;
												}
											}
											// if it has a className
											if(typeof(objChildNodes[intA].className)!='undefined'){
												if(objChildNodes[intA].className!=''){
													// split the classnames
													allClasses = objChildNodes[intA].className.split(' ');
													// for all classes
													for(var b=allClasses.length-1; b>=0; b--){
														// add class to stylesheet prototype
														newEntry = strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b] + ' {}\n';
														// add this style only if there's not double
														if(!this.isInStylesheet(newEntry) && !this.isClassBehaviour(newEntry)){
															// update the prefix
															this.styleSheet += newEntry;
															// if the last entry was a link
															if(objChildNodes[intA].nodeName=='A'){
																// repeat it four times with the mouseover states
																this.styleSheet += '\t' + strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b] + ':link,\n';
																this.styleSheet += '\t' + strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b] + ':visited {}\n';
																this.styleSheet += '\t' + strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b] + ':hover,\n';
																this.styleSheet += '\t' + strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b] + ':active {}\n';
																// and jump further in
																intRecursion += 1;
															}
														}
														// update the prefix
														classPrefix = objChildNodes[intA].nodeName.toLowerCase() + '.' + allClasses[b];
													}
												}
											}
											// if it has neither
											if(
												objChildNodes[intA].className=='' && 
												(objChildNodes[intA].id=='' || this.isFormElement(objChildNodes[intA])) && 
												objChildNodes[intA].nodeName.indexOf('text')<0 && 
												objChildNodes[intA].nodeName.indexOf('comment')<0
											){
												// add class to stylesheet prototype
												newEntry = strTabs + prefix + objChildNodes[intA].nodeName.toLowerCase() + ' {}\n';
												// add this style only if there's not double
												if(!this.isInStylesheet(newEntry)){
													this.styleSheet += newEntry;
													// if the last entry was a link
													if(newEntry.indexOf(' a {}')>-1){
														// repeat it four times with the mouseover states
														this.styleSheet += '\t' + newEntry.replace('a {}','a:link,');
														this.styleSheet += '\t' + newEntry.replace('a {}','a:visited {}');
														this.styleSheet += '\t' + newEntry.replace('a {}','a:hover,');
														this.styleSheet += '\t' + newEntry.replace('a {}','a:active {}');
														// and jump further in
														intRecursion += 1;
													}
												}
												// update the prefix
												tagPrefix = objChildNodes[intA].nodeName.toLowerCase();
											}
											// if it has childNodes
											if(objChildNodes[intA].childNodes.length>0){
												// update the prefix
												if(idPrefix){
													addPrefix = idPrefix + ' ';
												}else if(classPrefix){
													addPrefix = prefix + classPrefix + ' ';
												}else if(tagPrefix){
													addPrefix = prefix + tagPrefix + ' ';
												}
												// recurse
												this.getNodeClasses(objChildNodes[intA], intRecursion+1, addPrefix);
											}
										}
									}
		this.showNodeClasses	=	function(){
										var mss = classBehaviour.makeStylesheet;
										document.body.style.textAlign = 'left';
										document.body.style.background = '#ffffff none';
										document.body.style.color = '#000000';
										document.body.style.fontFamily = 'Sans Serif';
										document.body.style.fontSize = '12pt';
										mss.getNodeClasses(mss.rootObject, 0 , '');
										document.body.innerHTML = '<textarea style=""position:absolute;left:12.5%;top:12.5%;width:75%;height:75%;border:solid 1px #000000;"">' + mss.styleSheet + '</textarea>';
									}
	}
	// add this function to the classbehaviour object
	classBehaviour.makeStylesheet = new MakeStylesheet;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.makeStylesheet;
	
    // move a link's click event to the parent node
	// define this class behaviour
	function ClickOnParent(){
		/* properties */
		this.name 		= 	'clickOnParent';
		/* methods */
		this.start		=	function(node){
								// what node is the click supposed to go on?
								parentCount = parseInt(classBehaviour.utilities.getClassParameter(node, ""parent"", ""2""));
								targetNode = node;
								for(var a=0; a<parentCount; a++) targetNode = targetNode.parentNode;
								// get the target of the link
								linkTarget = node.href;
								// set the click event
								targetNode.onclick = this.clicked;
								// set the cursor
								targetNode.style.cursor = 'pointer';
							}
		/* events */
		this.clicked	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// what is the link
								linkTargets = objNode.getElementsByTagName('A');
								for(var a=0; a<linkTargets.length; a++) if(linkTargets[a].className.indexOf('clickOnParent')>-1) linkTarget = linkTargets[a].href;
								// go to the link
								document.location.href = linkTarget;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.clickOnParent = new ClickOnParent;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.clickOnParent;
	
// tabbed content
	// define this class behaviour
	function TabbedContent(){
		/* properties */
		this.name 		= 	'tabbedContent';
		/* methods */
		this.start		=	function(node){
								// get all tabs
								allTabs = node.getElementsByTagName('a');
								// store the most likely opened tab
								openedTab = allTabs[0];
								// for all tabs
								for(var a=0; a<allTabs.length; a++){
									// get the id this tab refers to
									tabId = allTabs[a].href.split('#')[1];
									// apply onclick events to the referred tab
									allTabs[a].onclick = this.open;
									// apply the starting state of the tab if needed
									if(allTabs[a].className.indexOf('closedTab')<0 && allTabs[a].className.indexOf('openedTab')<0) allTabs[a].className += ' closedTab';
									// apply the starting state of the referred content if needed
									document.getElementById(tabId).style.display = 'none';
									// if this tab is referred to in the page url, remember it as active
									if(document.location.href.indexOf(allTabs[a].href)>-1) openedTab = allTabs[a];
									// if this tab was manualy set
									if(allTabs[a].className.indexOf('openedTab')>-1) openedTab = allTabs[a];
								}
								// if there is a pager
								pager = document.getElementById(classBehaviour.utilities.getClassParameter(node, 'pagerId', 'none'));
								if(pager){
									// assign the events for the buttons
									pager.getElementsByTagName('a')[0].onclick = this.previous;
									pager.getElementsByTagName('a')[1].onclick = this.next;
								}
								// open the most likely first tab
								this.open(openedTab, true);
							}
		/* events */
		this.next		=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var tbd = classBehaviour.tabbedContent;
								// get the pager information
								pagerInfo = objNode.parentNode.parentNode.getElementsByTagName('span')[0].firstChild.nodeValue;
								// what is the current pagenumber
								currentPage = parseInt(pagerInfo.split('/')[0]);
								// how many pages are there
								totalPages = parseInt(pagerInfo.split('/')[1]);
								// what is the next page
								nextPage = (currentPage<totalPages) ? currentPage + 1 : 1 ;
								// what is the tabs strip
								tabStrip = document.getElementById(classBehaviour.utilities.getClassParameter(objNode.parentNode.parentNode, 'tabsId', 'none'));
								if(tabStrip){
									// get the relevant page from the tab strip
									targetTab = tabStrip.getElementsByTagName('a')[nextPage-1];
									// activate it's click
									tbd.open(targetTab);
								}
							}
		this.previous	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var tbd = classBehaviour.tabbedContent;
								// get the pager information
								pagerInfo = objNode.parentNode.parentNode.getElementsByTagName('span')[0].firstChild.nodeValue;
								// what is the current pagenumber
								currentPage = parseInt(pagerInfo.split('/')[0]);
								// how many pages are there
								totalPages = parseInt(pagerInfo.split('/')[1]);
								// what is the next page
								previousPage = (currentPage>1) ? currentPage - 1 : totalPages ;
								// what is the tabs strip
								tabStrip = document.getElementById(classBehaviour.utilities.getClassParameter(objNode.parentNode.parentNode, 'tabsId', 'none'));
								if(tabStrip){
									// get the relevant page from the tab strip
									targetTab = tabStrip.getElementsByTagName('a')[previousPage-1];
									// activate it's click
									tbd.open(targetTab);
								}
							}
		this.open		=	function(that, noAnimation){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								var tbd = classBehaviour.tabbedContent;								
		
								// INDEX THE TAB STATES
								// get all tabs
								var allTabs = objNode.parentNode.parentNode.getElementsByTagName('a');
								var prevTab = null;
								var pageNumber = 0;
								// find the current tab
								for(var a=0; a<allTabs.length; a++){
									// rememeber the previous tab
									if(allTabs[a].className.indexOf('openedTab')>-1) prevTab = allTabs[a];
									// count the new pagenumber
									if(allTabs[a]==objNode) pageNumber = a;
								}

								// if this is the current tab again
								if(prevTab!=objNode || noAnimation){
								
									// PREVIOUS TAB
									if(prevTab){
										// mark the previous tab as passive
										prevTab.className = prevTab.className.replace('openedTab', 'closedTab');
										// if the tab has an image
										tabImages = prevTab.getElementsByTagName('img');
										if(tabImages.length>0) tabImages[0].src = tabImages[0].src.replace('_active','_link');
										// id the previous tabbed content
										prevContentId = prevTab.href.split('#')[1];
									}
									
									// NEXT TAB
									// mark the next tab as active
									objNode.className = objNode.className.replace('closedTab', 'openedTab');
									// if the tab has an image
									tabImages = objNode.getElementsByTagName('img');
									if(tabImages.length>0) tabImages[0].src = tabImages[0].src.replace('_link','_active').replace('_hover','_active');
									// id the next tabbed content
									nextContentId = objNode.href.split('#')[1];
									
									// FADE ANIMATION
									isAnimated = (noAnimation) ? 'no' : classBehaviour.utilities.getClassParameter(objNode, 'animated', 'yes') ;
									if(isAnimated=='yes'){
										// make the previous tab float
										document.getElementById(prevContentId).style.position = 'absolute';
										document.getElementById(prevContentId).style.top = '0px';
										// make the next tab no float
										document.getElementById(nextContentId).style.position = 'relative';
										// order the animation
										classBehaviour.fader.crossFade(nextContentId, prevContentId, 0, 25, 10, null);
									}else{
										if(prevTab) document.getElementById(prevContentId).style.display = 'none';
										document.getElementById(nextContentId).style.display = 'block';
									}
									
									// PAGE NUMBER
									// update page-numbering
									pager = document.getElementById(classBehaviour.utilities.getClassParameter(objNode.parentNode.parentNode, 'pagerId', 'none'));
									if(pager){
										pager.getElementsByTagName('span')[0].firstChild.nodeValue = (pageNumber+1) + '/' + allTabs.length ;
									}

								}
								// cancel the jump to the anchor
								return false;
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.tabbedContent = new TabbedContent;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.tabbedContent;
	
// edit the order of items in select lists
	// define this class behaviour
	function EditSelect(){
		/* properties */
		this.name 		= 	'editSelect';
		/* methods */
		this.start		=	function(node){
								// was the click aimed at the select list
								if(node.nodeName=='SELECT'){
									node.className += ' from_' + node.id;
									node.ondblclick = this.clicked;
								}else{
									node.onclick = this.clicked;
								}
							}
		/* events */
		this.clicked	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// button settings
								moveSourceId = classBehaviour.utilities.getClassParameter(objNode, 'from', null);
								moveDestinationId = classBehaviour.utilities.getClassParameter(objNode, 'to', null);
								upSourceId = classBehaviour.utilities.getClassParameter(objNode, 'up', null);
								downSourceId = classBehaviour.utilities.getClassParameter(objNode, 'down', null);
								// get the parent nodes
								moveSource = document.getElementById(moveSourceId);
								upSource = document.getElementById(upSourceId);
								downSource = document.getElementById(downSourceId);
								moveDestination = document.getElementById(moveDestinationId);
								// if we're going to move a node
								if(moveSource && moveDestination){
									// get the source node
									sourceNode = (moveSource.selectedIndex>-1) ? moveSource.getElementsByTagName('option')[moveSource.selectedIndex] : null;
									// get the target node
									destinationNode = (moveDestination.selectedIndex>-1) ? moveDestination.getElementsByTagName('option')[moveDestination.selectedIndex] : null;
									// if there is a source node
									if(sourceNode){
										// remove the source node
										removedNode = moveSource.removeChild(sourceNode);
										// insert the source node before the desination node 
										if(destinationNode){
											moveDestination.insertBefore(removedNode, destinationNode.nextSibling);
										}else{
											moveDestination.appendChild(removedNode);
										}
									}
								}
								// if we're going to shift a node up
								if(upSource){
									// get the source node
									upNodes = upSource.getElementsByTagName('option');
									sourceNode = (upSource.selectedIndex>-1) ? upNodes[upSource.selectedIndex] : null;
									destinationNode = (upSource.selectedIndex>0) ? upNodes[upSource.selectedIndex-1] : null ;
									if(sourceNode && destinationNode){
										// remove the source node
										removedNode = upSource.removeChild(sourceNode);
										// insert it at its new place
										upSource.insertBefore(removedNode, destinationNode);
									}
								}
								// if we're going to shift a node down
								if(downSource){
									// get the source node
									downNodes = downSource.getElementsByTagName('option');
									sourceNode = (downSource.selectedIndex>-1) ? downNodes[downSource.selectedIndex] : null;
									destinationNode = (downSource.selectedIndex<downNodes.length-2) ? downNodes[downSource.selectedIndex+2] : null ;
									if(sourceNode && destinationNode){
										// remove the source node
										removedNode = downSource.removeChild(sourceNode);
										// insert it at its new place
										downSource.insertBefore(removedNode, destinationNode);
									}else if(sourceNode){
										// remove the source node
										removedNode = downSource.removeChild(sourceNode);
										// insert it at its new place
										downSource.appendChild(removedNode);
									}
								}
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.editSelect = new EditSelect;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.editSelect;
	
// Use a list of filenames as a selector
	// define this class behaviour
	function BinaryList(){
		/* properties */
		this.name 				= 	'binaryList';
		/* methods */
		this.start				=	function(node){
										// for all lists in the parent
										allLists = node.getElementsByTagName('ul');
										for(var a=0; a<allLists.length; a++){
											// if there is a file list, set the event handlers
											if(allLists[a].className.indexOf('files')>-1) this.fileEvents(allLists[a]);
											// if there is a basket list, set the event handlers
											if(allLists[a].className.indexOf('basket')>-1) this.basketEvents(allLists[a]);
											// if there are move buttons, set the event handlers
											if(allLists[a].className.indexOf('move')>-1) this.moveEvents(allLists[a]);
											// if there are sort buttons, set the event handlers
											if(allLists[a].className.indexOf('order')>-1) this.orderEvents(allLists[a]);
										}
									}
		this.fileEvents			=	function(node){
										// for all files in the list
										allItems = node.getElementsByTagName('a');
										for(var a=0; a<allItems.length; a++){
											// if this is a folder have it, fetch the url, otherwise process the thumbnail
											allItems[a].onclick 	= 	(allItems[a].className.indexOf('folder')>-1) ? 
																			this.folderActivate :
																				(allItems[a].className.indexOf('directory')>-1) ?
																					this.directorySelect :
																						this.fileSelect ;
											allItems[a].ondblclick 	= 	(allItems[a].className.indexOf('folder')>-1) ? 
																			this.folderActivate : 
																				(allItems[a].className.indexOf('directory')>-1) ? 
																					this.directorySelect :
																						this.fileActivate ;
										}
									}
		this.basketEvents		=	function(node){
										// get all items in the list
										allItems = node.getElementsByTagName('a');
										// for every item
										for(var a=0; a<allItems.length; a++){
											// reset its classname
											allItems[a].className = allItems[a].className.replace('active','link');
											// reset the click events
											allItems[a].onclick = this.fileSelect;
											allItems[a].ondblclick = this.basketActivate;
										}
									}
		this.moveEvents			=	function(node){
										// all buttons
										allButtons = node.getElementsByTagName('button');
										// give the right button the right event
										for(var a=0; a<allButtons.length; a++) 
											allButtons[a].onclick = (allButtons[a].className.indexOf('toLeft')>-1) ? this.removeFrom : this.addTo ;
									}
		this.orderEvents		=	function(node){
										// all buttons
										allButtons = node.getElementsByTagName('button');
										// give the right button the right event
										for(var a=0; a<allButtons.length; a++)
											allButtons[a].onclick = (allButtons[a].className.indexOf('orderUp')>-1) ? this.moveUp : this.moveDown ;
									}
		this.fillFolder			=	function(documentObject, referedNode, documentText){
										bls = classBehaviour.binaryList
										// put the returned text in the refered node
										rootNode = referedNode.parentNode.parentNode;
										rootNode.innerHTML = documentText.replace('<?xml version=""1.0""?>','').replace('<root>','').replace('<root>','');
										// (re)apply its event-handlers
										bls.start(rootNode.parentNode);
									}
		this.findList			=	function(rootNode,className){
										foundList = null;
										for(var a=0; a<rootNode.childNodes.length; a++) 
											if(rootNode.childNodes[a].className) 
												if(rootNode.childNodes[a].className.indexOf(className)>-1) 
													foundList = rootNode.childNodes[a];
										return foundList;
									}
		this.reportBasket		=	function(basketNode){
										// locte the relevant nodes
										rootNode = basketNode.parentNode;
										inputField = (rootNode.getElementsByTagName('input').length>0) ? rootNode.getElementsByTagName('input')[0] : null;
										// find the active nodes in the basket list and note their id's
										inputValue = '';
										allNodes = basketNode.getElementsByTagName('a');
										for(var a=0; a<allNodes.length; a++) inputValue += allNodes[a].id + ',';
										// write those id's to the input field
										inputField.value = inputValue;
									}
		/* events */
		this.moveUp				=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										rootNode = objNode.parentNode.parentNode.parentNode;
										// find the basket list
										basketList = classBehaviour.binaryList.findList(rootNode, 'basket');
										// find the active item
										activeNode = null;
										allNodes = basketList.getElementsByTagName('a');
										for(var a=0; a<allNodes.length; a++) if(allNodes[a].className.indexOf('active')>-1) activeNode = allNodes[a];
										// if there is an active item
										if(activeNode){
											// store the previous node
											previousNode = classBehaviour.utilities.previousNode(activeNode.parentNode);
											// don't replace the same node
											if(previousNode!=activeNode.parentNode){
												// remove and store the active node
												removedNode = basketList.removeChild(activeNode.parentNode);
												// add it above the previous node
												basketList.insertBefore(removedNode, previousNode);
												// update the basket's selection
												classBehaviour.binaryList.reportBasket(basketList);
											}
										}
									}
		this.moveDown			=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										rootNode = objNode.parentNode.parentNode.parentNode;
										// find the basket list
										basketList = classBehaviour.binaryList.findList(rootNode, 'basket');
										// find the active item
										activeNode = null;
										allNodes = basketList.getElementsByTagName('a');
										for(var a=0; a<allNodes.length; a++) if(allNodes[a].className.indexOf('active')>-1) activeNode = allNodes[a];
										// if there is an active item
										if(activeNode){
											// store the previous node
											nextNode = classBehaviour.utilities.nextNode(activeNode.parentNode,2);
											// don't replace the same node
											if(nextNode!=activeNode.parentNode){
												// remove and store the active node
												removedNode = basketList.removeChild(activeNode.parentNode);
												// add it above the previous node
												basketList.insertBefore(removedNode, nextNode);
											}else{
												// remove and store the active node
												removedNode = basketList.removeChild(activeNode.parentNode);
												// add it above the previous node
												basketList.appendChild(removedNode);
											}
											// update the basket's selection
											classBehaviour.binaryList.reportBasket(basketList);
										}
									}
		this.removeFrom			=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										rootNode = objNode.parentNode.parentNode.parentNode;
										// find the basket list
										basketList = classBehaviour.binaryList.findList(rootNode, 'basket');
										// find the active item
										activeNode = null;
										allNodes = basketList.getElementsByTagName('a');
										for(var a=0; a<allNodes.length; a++) if(allNodes[a].className.indexOf('active')>-1) activeNode = allNodes[a];
										// if there is an active item
										if(activeNode){
											//remove it
											removedNode = basketList.removeChild(activeNode.parentNode);
											// update the basket's selection
											classBehaviour.binaryList.reportBasket(basketList);
										}
									}
		this.addTo				=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										rootNode = objNode.parentNode.parentNode.parentNode;
										// find the file list
										filesList = classBehaviour.binaryList.findList(rootNode, 'files');
										// find the active item
										activeNode = null;
										allNodes = filesList.getElementsByTagName('a');
										for(var a=0; a<allNodes.length; a++) if(allNodes[a].className.indexOf('active')>-1) activeNode = allNodes[a];
										// if there is an active item
										if(activeNode){
											//remove it
											classBehaviour.binaryList.fileActivate(activeNode);
										}
									}
		this.directorySelect	=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										bls = classBehaviour.binaryList;
										// find your way to the root of the list
										listNode = objNode.parentNode.parentNode;
										while(listNode.className.indexOf('list')<0) listNode = listNode.parentNode.parentNode;
										rootNode = listNode.parentNode;
										// mark the other nodes as not active
										otherNodes = listNode.getElementsByTagName('a');
										for(var a=0; a<otherNodes.length; a++){
											otherNodes[a].className = otherNodes[a].className.replace('active','link')
										}
										// mark the node as active
										if(rootNode.getElementsByTagName('input').length>0) rootNode.getElementsByTagName('input')[0].value = objNode.id;
										objNode.className = (objNode.className.indexOf('active')<0) ? objNode.className.replace('link', 'active') : objNode.className.replace('active', 'link') ;
										// get the next node of the link
										nextNode = classBehaviour.utilities.nextNode(objNode);
										// if there is a next node
										if(nextNode!=objNode){
											if(nextNode.style.display=='block'){
												objNode.className = (objNode.className.indexOf('opened')<0) ? objNode.className + ' closed' : objNode.className.replace('opened', 'closed');
												nextNode.style.display = 'none';
											}else{
												objNode.className = (objNode.className.indexOf('closed')<0) ? objNode.className + ' opened' : objNode.className.replace('closed', 'opened');
												nextNode.style.display = 'block';
											}
										}
										// cancel the click
										return false;
									}
		this.fileSelect			=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										listNode = objNode.parentNode.parentNode;
										rootNode = listNode.parentNode;
										// find the basket list
										basketList = classBehaviour.binaryList.findList(rootNode, 'basket');
										// for all files in the list
										filesList = listNode.getElementsByTagName('a');
										for(var a=0; a<filesList.length; a++){
											// check if this is the active one
											if(filesList[a]==objNode){
												// set the item to active
												filesList[a].className = filesList[a].className.replace('link','active');
												// if there is an output field, store it there to
												if(rootNode.getElementsByTagName('input').length>0 && basketList==null) rootNode.getElementsByTagName('input')[0].value = objNode.id;
											}else{
												// set the item to passive
												filesList[a].className = filesList[a].className.replace('active','link');
											}
										}
										// if there is a thumbnail
										previewImages = rootNode.getElementsByTagName('img');
										if(previewImages.length>0) if(previewImages[0].className.indexOf('preview')>-1) previewImages[0].src = objNode.href;
										// cancel the click
										return false;
									}
		this.folderActivate		=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										bls = classBehaviour.binaryList
										// get the xml file referenced, pass the fetcher the object and an onload function
										url = objNode.href;
										loadHandler = bls.fillFolder;
										progressHandler = null;
										post = null;
										eventObject = objNode;
										// start the ajax request
										classBehaviour.xmlDoc.addRequest(url, loadHandler, progressHandler, post, eventObject);
										// cancel the click
										return false;
									}
		this.fileActivate		=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// get the parent node with all controls
										listNode = objNode.parentNode.parentNode;
										rootNode = listNode.parentNode;
										// find the basket list
										basketList = classBehaviour.binaryList.findList(rootNode, 'basket');
										// if there is an output field, store it there to
										if(rootNode.getElementsByTagName('input').length>0) rootNode.getElementsByTagName('input')[0].value = objNode.id;
										// if there is a file basket
										if(basketList){
											// copy the list node
											clonedNode = objNode.parentNode.cloneNode(true);
											// add it to the basket
											basketList.appendChild(clonedNode);
											// reset its events
											classBehaviour.binaryList.basketEvents(basketList);
											// update the basket's selection
											classBehaviour.binaryList.reportBasket(basketList);
										}
										// cancel the click
										return false;
									}
		this.basketActivate		=	function(that){
										var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
										// find the basket list
										basketList = objNode.parentNode.parentNode;
										// remove this node
										removedNode = basketList.removeChild(objNode.parentNode);
										// update the basket's selection
										classBehaviour.binaryList.reportBasket(basketList);
										// cancel the click
										return false;
									}
	}
	// add this function to the classbehaviour object
	classBehaviour.binaryList = new BinaryList;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.binaryList;
	
// move a link's click event to the parent node
	// define this class behaviour
	function TextOptionList(){
		/* properties */
		this.name 		= 	'textOptionList';
		this.index		=	0;
		/* methods */
		this.start		=	function(node){
								// give this node an id if it hasn't allready
								this.index += 1;
								node.id = (node.id) ? node.id : this.name + this.index ;
								// get all buttons
								allButtons = node.getElementsByTagName('button');
								for(var a=0; a<allButtons.length; a++){
									// the remove button should remove the last item from the list
									if(allButtons[a].className.indexOf('remove')>0) allButtons[a].onclick = this.clearLast;
								}
							}
		/* events */
		this.clearLast	=	function(that){
								var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
								// get the parent node
								rootNode = objNode.parentNode.parentNode.parentNode;
								// get the list items
								listNode = rootNode.getElementsByTagName('ul')[0];
								listItems = listNode.getElementsByTagName('li');
								// remove the last list item
								if(listItems.length>0){
									removedChild = listNode.removeChild(listItems[listItems.length-1]);
								}
								// clear the id
								rootNode.getElementsByTagName('input')[0].value = '';
							}
	}
	// add this function to the classbehaviour object
	classBehaviour.textOptionList = new TextOptionList;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.textOptionList;
	
	// define this class behaviour
	function TextOptionSelect(){
		/* properties */
		this.name 			= 	'textOptionSelect';
		this.index			=	0;
		/* methods */
		this.start			=	function(node){
									// give every row in this table an event handler
									allRows = node.getElementsByTagName('tr');
									for(var a in allRows){
										if(allRows[a].className){
											if(allRows[a].className.indexOf('link')>-1){
												allRows[a].onclick = this.pickValue;
												allRows[a].onmouseover = this.highlightRow;
												allRows[a].onmouseout = this.unHighlightRow;
											}
										}
									}
								}
		/* events */
		this.pickValue		=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									// find the element containing the value of this selection
									inputValue = objNode.getElementsByTagName('input')[0].value;
									inputId	= objNode.getElementsByTagName('input')[0].id;
									// find out what the id of the target element is
									targetId = document.location.href.split('referid=')[1].split('&')[0];
									// fill it with the value of this selected element
									parent.document.getElementById(targetId).getElementsByTagName('input')[0].value = inputId;
									// clear the displayed list
									listNode = parent.document.getElementById(targetId).getElementsByTagName('ul')[0];
									allListItems = listNode.getElementsByTagName('li');
									for(var a=0; a<allListItems.length; a++)  listNode.removeChild(allListItems[a]);
									// add the contents of the selection to displayed list
									newListItem = parent.document.createElement('li');
									newListText = parent.document.createTextNode(inputValue);
									newListItem.appendChild(newListText);
									listNode.appendChild(newListItem);
									// close the popup
									parent.classBehaviour.openLayerPopUp.hide(parent.document.getElementById('popUpWithIframe').getElementsByTagName('a')[0]);
								}
		this.highlightRow	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									//change the class
									objNode.className = objNode.className.replace('link', 'hover');
									objNode.style.cursor = 'pointer';
								}
		this.unHighlightRow	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									//change the class
									objNode.className = objNode.className.replace('hover', 'link');
									objNode.style.cursor = 'default';
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.textOptionSelect = new TextOptionSelect;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.textOptionSelect;

	// define this class behaviour
	function Postback(){
		/* properties */
		this.name 			= 	'postBack';
		/* methods */
		this.start		=	function(node){
								node.onclick = this.clicked;
							}
		/* events */
		this.clicked	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									document.forms[0].submit();
									return false;
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.postBack = new Postback;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.postBack;

	// define this class behaviour
	function PostChange(){
		/* properties */
		this.name 			= 	'postChange';
		/* methods */
		this.start		=	function(node){
								node.onchange = this.changed;
								node.onselect = this.changed;
							}
		/* events */
		this.changed	=	function(that){
									var objNode = (typeof(this.nodeName)=='undefined') ? that : this ;
									document.forms[0].submit();
									return false;
								}
	}
	// add this function to the classbehaviour object
	classBehaviour.postChange = new PostChange;
	classBehaviour.handlers[classBehaviour.handlers.length] = classBehaviour.postChange;

	
// STARTUP-SEQUENCE
// start the parsing of classes
classBehaviour.parseDocument();
"
                , container.WimRepository);
        }
    }
}
