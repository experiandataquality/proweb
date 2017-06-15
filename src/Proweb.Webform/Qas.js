/// QAS Pro Web > (c) Experian > www.edq.com
/// Javascript Utility Library

// Base variable
var Qas = {};

// Name: Qas.getElement
// Desc: Get an element - allows either strings or objects to be passed in
// Args: elem   -   Element to get - either the element's id or the element itself ( string / obj )
// Rtns: Either the element or, if not found, null.

Qas.getElement = function( elem )
{      
	if ( typeof( elem ) == 'object' )
	{
		return elem;
	}
	
	if ( typeof( elem ) == 'string' )
	{        
		try
		{
			var _elem = document.getElementById( elem );
			return _elem;
		}
		catch( x )
		{}
	}
	
	return null;
};

// Name: Qas.dumpArray
// Desc: Debug function to write out the contents of an array to a string.
//       Recursively expands any inner arrays.
// Args: a  -   The array to dump
// Rtns: String representing contents of 'a'

Qas.dumpArray = function( a )
{
	var sResult = "[ ";
	
	for ( var i = 0; i < a.length; i++ )
	{
		if ( i != 0 )
		{
			sResult += ", ";
		}
		
		if ( a[i] instanceof Array )
		{
			sResult += Qas.dumpArray( a[i] );
		}
		else
		{
			sResult += a[i];
		}
	}
	
	sResult += " ]";
	
	return sResult;
};

// Name: Qas.setText
// Desc: Set the text of an element - if it has a 'value' property, try to set that.
//       Otherwise, try to set it's innerHTML.
// Args: elem   -   the element in question
//       sText  -   the text to set
//       sPad   -   ( optional ) surround the element with this string
// Rtns: void

Qas.setText = function( elem, sText, sPad )
{
	var bSet = false;
	
	if ( sPad )
	{
		sText = sPad + sText + sPad;
	}
	
	if ( typeof(elem.value) != 'undefined' )
	{        
		try 
		{
			elem.value = sText;
			bSet = true;
		}
		catch( x )
		{
			bSet = false;
		}
		
	}
	if ( !bSet && ( typeof(elem.innerHTML) == 'string' ) )
	{
		elem.innerHTML = sText;
	}
	else
	{
		throw "Qas - setText() - unable to set text of elem '" + elem.id + "' to '" + sText + "'";
	}     
};

// Event Namespace
Qas.Event = {};

// Array to keep track of events we've registered ( IE only )
Qas.Event._aEvents = [];

// Name: Qas.addEventListener
// Desc: Add an event handler to an element
// Args: elem       -  Element to add the event to ( element )
//       sEvent     -  Name of the event to register for ( W3C style - e.g. 'click', 'mouseover' - No 'on's ) ( string )
//       fFunction  -  Function to call when event fires  ( function )
//       oContext   -  Context to execute function in  ( object )
//       aArgs      -  Array of arguments to pass to function ( array )
// Rtns: void
// Notes: When the event fires, the function 'fFunction' will be called with an event object as it's first argument,
//        followed by the items in aArgs.

Qas.Event.addEventListener = function( elem, sEvent, func, oContext, aArgs )
{   
	elem = Qas.getElement( elem );
	
	if ( elem.addEventListener )  //  W3C ( Mozilla, Opera, Safari )
	{
		elem.addEventListener( sEvent, Qas.Event.wrapEventFunction( func, oContext, aArgs ), false );
	}
	else if ( elem.attachEvent )  //  IE
	{
		if ( sEvent == "keypress" ) { sEvent = "keydown"; }
		
		var sIeEvent = "on" + sEvent;
		elem.attachEvent( sIeEvent, Qas.Event.wrapEventFunction( func, oContext, aArgs ) );
	}
	else
	{
		throw "Qas - addEventlistener() - Unable to register event '" + sEvent + "' on element with id '" + elem.id + "'";
	}
	
	// IE only - keep a list of all the events we've registered
	// Manually detach the events when the window 'unloads' to prevent IE memory leaks
	if ( elem.attachEvent )
	{
		// Add the event to our list
		Qas.Event._aEvents.push( { _elem: elem, _sEvent: sEvent, _func: func } );
		
		if ( Qas.Event._aEvents.length == 1 )
		{
			Qas.Event.addEventListener( window, 'unload', Qas.Event._detachEvents, this, [Qas.Event._aEvents] );  
		} 
	}    
};

// Name: Qas._detachEvents
// Desc: Manually unload all event handlers we've set
// Args: e       -  event ( unused )
//       aEvents -  Array of events to unload
// Rtns: void

Qas.Event._detachEvents = function( e, aEvents )
{
	var obj = null;
	
	for ( var i = 0; i < aEvents.length; i++ )
	{
		obj = aEvents[i];
			  
		if ( obj._elem.detachEvent )
		{
			if ( obj._sEvent == "keypress" ) { obj._sEvent = "keydown"; }
			var sIeEvent = "on" + obj._sEvent;
			
			obj._elem.detachEvent( sIeEvent, obj._func );
		}       
	}
};


// Name: Qas.wrapEventFunction
// Desc: Wrap an event handling function so it can be called in a particular context & with arguments
//       Also, make sure the event obj is passed to the function correctly
// Args: fnFunction  -  Function to wrap
//       oContext   -  Context to execute function in
//       aArgs      -  Array of arguments to pass to function
// Rtns: New function which wraps 'fnFunction'

Qas.Event.wrapEventFunction = function( fnFunction, oContext, aArgs )
{   
	return function( event ) { fnFunction.apply( oContext, [Qas.Event.getEvent(event)].concat( aArgs ) ) };
}

// Name: Qas.getEvent
// Desc: Get the event object & make an attempt to standardise it
// Args: e  -   Event object ( maybe )
// Rtns: 'standardised'(ish) event object

Qas.Event.getEvent = function( e )
{        
	if ( !e || e.recordset || e.recordset === null  ) // IE
	{
		e = e || window.event;
		
		if ( !e )
		{
			throw "Qas Event - getEvent() - unable to locate event object"
		}
		
		// Rename the 'srcElement' to 'target' so it matches other browsers
		e.target = e.srcElement || null;
		
		// Set the button property to match other browsers
		if ( e.button == 1 )      { e.button = 0; }
		else if ( e.button == 2 ) { e.button = 1; }
		else if ( e.button == 4 ) { e.button = 2; }
			
		return e;
	}
	else 
	{
		// Mozilla, Safari, Opera ...
		
		// Safari sometimes returns text nodes rather than their parent
		// If the target is a text node, set it to it's parent
		if ( e.target.nodeType && e.target.nodeType == 3 )
		{
			e.target = e.target.parentNode;
		}
		
		return e;
	}
}

// Name: Qas.Event.stopEvent
// Desc: Attempt to stop an event's propagation
// Args: e  -   Event object
// Rtns: void

Qas.Event.stopEvent = function( e )
{
	if ( e.stopPropagation )    // W3C
	{
		e.stopPropagation();
	}
	if ( e.cancelBubble )       // IE
	{
		e.cancelBubble = true;
	}
}

// Name: Qas.Event.preventDefault
// Desc: Attempt to prevent the default action of an event
// Args: e  -   Event object
// Rtns: void

Qas.Event.preventDefault = function( e )
{
	if ( e.preventDefault )     // W3C
	{
		e.preventDefault();
	}
	if ( e.returnValue )        // IE
	{
		e.returnValue = false;
	}
}

