/// QAS Pro Web > (c) Experian > www.edq.com
/// Intranet > Rapid Addressing > Standard > RapidAddress
/// Javascript to control multiple dataplus viewing


//===================================================================
//  Multi Dataplus Control
//===================================================================


if ( typeof(window.Qas) == "undefined" )
{
	throw "Qas MultiDataplus Control - Base variable undefined - is 'Qas.js' included?";
}

// Define UI namespace if necessary
Qas.UI = Qas.UI || {};

// Constructor
Qas.UI.MultiDataplusControl = function()
{   
	this._name = "Qas MultiDataplus Control";   // Name - useful for debugging
	this._aGroups = {};                         // Groups ( as a hash ) in this control
	this._aGroupNames = [];                     // Array of group names in this control
};

// Instance methods
Qas.UI.MultiDataplusControl.prototype = {

	//================================
	//  Public functions
	//================================

	// Name: addControls
	// Desc: Add a set of controls to this control
	// Args: sGroup     -   Name of the group these elements control ( string )
	//       elemFwd    -   Element to click to increment this group display ( string / object )
	//       elemBck    -   Element to click to decrement this group display ( string / object )
	//       elemRtn    -   Element to determine whether to return the currently
	//                      displayed items ( should be a HTML checkbox ) ( string / object )
	// Rtns: void 
		 
	addControls:    function( sGroup, elemFwd, elemBck, elemRtn, elemIdx )
					{   
						var oGroup = this._addGroup( sGroup );
						
						oGroup._elemFwd = Qas.getElement( elemFwd );
						oGroup._elemBck = Qas.getElement( elemBck );
						oGroup._elemRtn = Qas.getElement( elemRtn );
						oGroup._elemIdx = Qas.getElement( elemIdx );
						
						if ( oGroup._elemFwd == null || oGroup._elemBck == null || oGroup._elemRtn == null || oGroup._elemIdx == null )
						{
							throw this._name + " - addControls() - Group " + 
									sGroup + " - control elements not specified";
						}
						
						// Register events
						Qas.Event.addEventListener( oGroup._elemFwd, "click", this._increment, this, [oGroup] );
						Qas.Event.addEventListener( oGroup._elemBck, "click", this._decrement, this, [oGroup] );
						Qas.Event.addEventListener( oGroup._elemRtn, "click", this._returnThis, this, [oGroup] );
					},
	
	// Name: addItems
	// Desc: Add a set of items to this control
	// Args: _sGroup    -   Name of the group these items are a part of ( string )
	//       _iLine     -   Line of the address on which these items appear ( int )
	//       _elem      -   Element which should display the items e.g. span, p, div ( string / object )
	//       ...        -   The rest of the arguments are the items to add ( strings )
	// Rtns: void 
	
	addItems:   function( _sGroup, _iLine, _elem /* Variable number of items */ )
				{
					var oGroup = this._addGroup( _sGroup );
					
					var _aItems = new Array();
					
					// Grab the rest of the args & populate items array
					for ( var i = 3; i < arguments.length; ++i )
					{
						_aItems.push( arguments[i] );
					}
					
					if ( _aItems.length == 0 ) { throw this._name + " - addItems() - No items to add"; }
					
					if ( _aItems.length > oGroup._iMaxItems ) { oGroup._iMaxItems = _aItems.length; }
					
					// Set initial values for display
					Qas.setText( Qas.getElement( _elem ), _aItems[0] );
					if ( oGroup._elemRtn ) { oGroup._elemRtn.checked = true; }
					
					var sIndexText = "1 / " + oGroup._iMaxItems;
					
					if ( oGroup._elemIdx )
					{ 
						Qas.setText( oGroup._elemIdx, sIndexText, "&nbsp;" ); 
					}
					
					// Add this set to the group
					oGroup._aSets.push( { elem: _elem, aItems: _aItems, iLine: _iLine } );
					
					// Build 'return' array if necessary
					if ( _aItems.length > oGroup._aReturn.length )
					{
						oGroup._aReturn = new Array();
						
						for( var j = 0; j < _aItems.length; j++ )
						{
							oGroup._aReturn.push(true);
						} 
					}
					
				},
	
	// Name: getResult
	// Desc: Get a formatted string representing this line
	// Args: iLine        -  Index of line to get ( int )
	//       sElemSep     -  ( optional ) String to use to separate elements - default ',' ( string )
	//       sDataplusSep -  ( optional ) String to use to separate multiple dataplus items - default '|' ( string )
	// Rtns: String containing formatted line  
							  
	getResult:  function( iLine, sElemSep, sDataplusSep )
				{
					var aDataplusStrings = new Array();
					var sResult = "";
					
					if ( sElemSep == null )
					{
						sElemSep = ",";
					}
					if ( sDataplusSep == null )
					{
						sDataplusSep = "|";
					}

					for ( var iGroup = 0; iGroup < this._aGroupNames.length; iGroup++ )
					{
						var oGroup = this._aGroups[ this._aGroupNames[iGroup] ];
						
						for ( var iSet = 0; iSet < oGroup._aSets.length; iSet++ )
						{
							var obj = oGroup._aSets[iSet];
							var aStrings = new Array();
							
							if ( obj.iLine == iLine )
							{
								for ( var iItem = 0; iItem < obj.aItems.length; iItem++ )
								{
									if ( oGroup._aReturn[iItem] == true )
									{
										aStrings.push( obj.aItems[iItem] );
									}
								}
								
								aDataplusStrings.push( aStrings );
							}
						}
					}
					
					for ( var i = 0; i < aDataplusStrings.length; i++ )
					{
						var aStrings = aDataplusStrings[i];
						
						if ( i != 0 && aStrings.length > 0 )
						{
							sResult += sElemSep + " ";
						}
						
						for ( var iString = 0; iString < aStrings.length; iString++ )
						{
							if ( iString != 0 )
							{
								sResult += sDataplusSep;
							}
							
							sResult += aStrings[iString];
						}
					}
					
					aDataplusStrings = null;
					
					return sResult;
				},
	
	
	//===========================================
	// Private functions
	//===========================================
	
	// Name: _addGroup
	// Desc: Add a group to this control
	// Args: sGroup     -   Name of group to add ( string )
	// Rtns: Object representing the group  
	 
	_addGroup:  function( sGroup )
				{
					if ( typeof( this._aGroups[sGroup] ) == 'undefined' )
					{
						// Create group object
						this._aGroups[sGroup] = {};
												
						this._aGroups[sGroup]._iCurrentPos = 0;     // Current position we're looking at
						this._aGroups[sGroup]._aSets = [];          // Sets of data in this group
						this._aGroups[sGroup]._aReturn = [];        // Array of bool - Whether to return items
						this._aGroups[sGroup]._iMaxItems = 0;
						
						this._aGroupNames.push( sGroup );
					}
					
					return ( this._aGroups[sGroup] );                                       
				},
	
	// Name: _increment ( Event handler - fired when "forward" element clicked )
	// Desc: If possible, increment the display of items in a group
	// Args: e      -  event obj ( not used here )
	//       oGroup -  Object representing the group to increment
	// Rtns: void
	
	_increment: function( e, oGroup )
				{  
					var objSet = null;
					var bIncrement = false;
					var iLength = 0;
					
					if ( this._canIncrement( oGroup ) )
					{
						// Loop through all sets in this group & try to increment them
						for ( i = 0; i < oGroup._aSets.length; ++i )
						{
							objSet = oGroup._aSets[i];
							
							if ( ( oGroup._iCurrentPos + 1 ) < objSet.aItems.length )
							{    
								Qas.setText( Qas.getElement( objSet.elem ), objSet.aItems[oGroup._iCurrentPos + 1] );
								oGroup._elemRtn.checked = oGroup._aReturn[oGroup._iCurrentPos + 1];
							}
							else
							{
								Qas.setText( Qas.getElement( objSet.elem ), "" );
								oGroup._elemRtn.checked = oGroup._aReturn[oGroup._iCurrentPos + 1];
							}
						}
						
						// If we managed to increment, edit the index text
						oGroup._iCurrentPos++; 
						var sIndexText = (oGroup._iCurrentPos + 1) + " / " + oGroup._iMaxItems; 
						Qas.setText( oGroup._elemIdx, sIndexText, "&nbsp;" ); 
					
					}
					
					// Stop event bubbling any further
					Qas.Event.stopEvent(e);
				},
	
	// Name: _decrement ( Event handler - fired when "backward" element clicked )
	// Desc: If possible, decrement the display of items in a group
	// Args: e      -  event obj ( not used )
	//       oGroup -  Object representing the group to decrement
	// Rtns: void           
	_decrement: function( e, oGroup )
				{
					var objSet = null;
					
					if ( this._canDecrement( oGroup ) )
					{
					
						// Loop through all sets in this group & try to decrement them
						for ( i = 0; i < oGroup._aSets.length; ++i )
						{
							objSet = oGroup._aSets[i];
							
							if ( typeof( objSet.aItems[oGroup._iCurrentPos - 1] ) != 'undefined' )
							{
								Qas.setText( Qas.getElement( objSet.elem ), objSet.aItems[oGroup._iCurrentPos - 1] );
								oGroup._elemRtn.checked = oGroup._aReturn[oGroup._iCurrentPos - 1];
							}
							else
							{
								Qas.setText( Qas.getElement( objSet.elem ), "" );
								oGroup._elemRtn.checked = oGroup._aReturn[oGroup._iCurrentPos - 1];
							}
						}
						
						// If we managed to decrement, edit the index text
						oGroup._iCurrentPos--;
						var sIndexText = (oGroup._iCurrentPos + 1) + " / " + oGroup._iMaxItems; 
						Qas.setText( oGroup._elemIdx, sIndexText, "&nbsp;" ); 

					}
					
					// Stop event bubbling any further
					Qas.Event.stopEvent(e);
				},
				
	_canIncrement : function( oGroup )
					{      
						var objSet = null;
						var bCanIncrement = false;
						
						// Loop through all sets in this group & try to increment them
						for ( i = 0; i < oGroup._aSets.length; ++i )
						{
							objSet = oGroup._aSets[i];
							
							if ( ( oGroup._iCurrentPos + 1 ) < objSet.aItems.length )
							{    
								bCanIncrement = true;
								break;
							}
						}
						
						return bCanIncrement;
					},
					
	_canDecrement : function( oGroup )
					{
						if ( ( oGroup._iCurrentPos - 1 ) >= 0 )
						{
							return true;
						}
						
						return false;
					},
	
	// Name: _returnThis ( Event handler - fired when "return this" checkbox clicked )
	// Desc: Remember to return / not return the items currently displayed
	// Args: e      -  event obj ( not used )
	//       oGroup -  Object representing the group to increment
	// Rtns: void    
			
	_returnThis : function( e, oGroup )
				  {
						oGroup._aReturn[oGroup._iCurrentPos] = oGroup._elemRtn.checked;
						
						Qas.Event.stopEvent(e);
				  }
				  
};


// End of MultiDPCtrl.js
