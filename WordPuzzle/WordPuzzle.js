var draggedRect, layer, stage, check, x1, y1, x2, y2, cHeight, cWidth;
  
$(document).ready(function(){
  cHeight = $('#container').height();
  cWidth = $('#container').width();
  
  $('#horizontal').hide();
  $('#vertical').hide();
  $('#diagonal').hide();
  
  layer = new Kinetic.Layer();
  stage = new Kinetic.Stage({
	container: 'container',
	width: cWidth,
	height: cHeight
  });
  
  stage.add(layer);
  
  var isDragging = false;
  
  CallService();
  
  $(stage.getContent()).on('touchstart mousedown', function(event){
	x1 = GetX();
	y1 = GetY();
	draggedRect = CurvedRectangle(x1, y1, 0, cWidth * 0.03, 0);
	isDragging = true;
  });

  $(stage.getContent()).on('touchmove mousemove', function(event, context){
	if(!isDragging)
	  return;
	else
	  draggedRect.remove();
	
	var w = stage.getPointerPosition().x - x1;
	var h = stage.getPointerPosition().y - y1;
	var theta = Math.atan(h/w);
	  
	if(w < 0)
	  theta = Math.PI + theta;
		
	else if(h < 0 && w >= 0)
	  theta = 2 * Math.PI + theta;
		
	draggedRect = CurvedRectangle(x1, y1, Math.sqrt(w*w + h*h), cWidth * 0.03, theta);
  });

  $(stage.getContent()).on('touchend mouseup', function(){
	isDragging = false;
	x2 = GetX();
	y2 = GetY();
	
	if(!CheckSolution()){
	  var w = x2 - x1;
	  var h = y2 - y1;
	  var theta = Math.atan(h/w);
	  
	  if(w < 0)
	    theta = Math.PI + theta;		
	  else if(h < 0 && w >= 0)
	    theta = 2 * Math.PI + theta;
		
	  draggedRect.remove();
	  draggedRect = CurvedRectangle(x1, y1, Math.sqrt(w*w + h*h), cWidth * 0.03, theta);
	  layer.draw();
	}
	else{
	  draggedRect.remove();
      layer.draw();
	}
  });
  
  $('#container').mouseout(function(){
    if(isDragging){
	  isDragging = false;
	  draggedRect.remove();
	  layer.draw();	  
	}
  });
});

function CallService(){
  $.ajax({
    type: 'GET',
	url: 'http://localhost:3000/wordpuzzlegenerator.svc/GenerateGrid',
	contentType: 'application/json; charset=utf-8',
	dataType: 'json',
	processData: false,
	success: function(data){
	  check = data.Check;
	  DrawGrid(data.Grid);
	  ShowHints(data.Meanings);
	},
	error: function(xhr){alert(xhr);}
  });
}

function DrawGrid(grid){
  var letter;
  var refX = cWidth * 0.02;
  var refY = cHeight * 0.02;
  
  for(var i = 0; i < 100; i++){
	if(i > 0 && i % 10 == 0)
	  refY = refY + cHeight * 0.1;
	
    letter = new Kinetic.Text({
	  x: refX + cWidth * 0.1 * (i % 10),
	  y: refY,
	  text: grid[i],
	  fontSize: cWidth * 0.06,
	  fill: 'black'
    });  
	
    layer.add(letter);
  }
  layer.draw();
}

function ShowHints(hints){
  var hasH = false;
  var hasV = false;
  var hasD = false;
  var d, l, m;
	  
  for(var i = 0; i < 10; i++){
	d = hints[i].split('|')[0];
	l = hints[i].split('|')[1].length;
	m = hints[i].split('|')[2];
		
	if(d == '0' || d == '1'){
	  hasH = true;
	  $('#horizontal').append(Meaning(i, l, m));
	}
		
	else if(d == '2' || d == '3'){
	  hasV = true;
	  $('#vertical').append(Meaning(i, l, m));
	}
		
	else{
	  hasD = true;
	  $('#diagonal').append(Meaning(i, l, m));
	}
	
	$('ul').css('font-size', cWidth * 0.06);
	$('li').css('font-size', cWidth * 0.05);
  }
	  
  if(hasH) $('#horizontal').show();
  if(hasV) $('#vertical').show();
  if(hasD) $('#diagonal').show();
}

function Meaning(i, l, m){
  return ('<li id=mn' + i + '>' + m + ' (' + l +')' + '</li>');
}

function GetX(){
  return (Math.floor(Math.abs(stage.getPointerPosition().x - (cWidth * 0.02)) / (cWidth * 0.1)) * cWidth * 0.1) + (cWidth * 0.04);
}

function GetY(){
  return (Math.floor(Math.abs(stage.getPointerPosition().y - (cHeight * 0.02)) / (cHeight * 0.1)) * cHeight * 0.1) + (cHeight * 0.05);
}

function CurvedRectangle(mouseX, mouseY, length, radius, theta){
  var roundRect = new Kinetic.Shape({
	sceneFunc: function(context){
	  context.beginPath();		  
	  if (theta !== 'undefined'){
		context.arc(mouseX, mouseY, radius, (Math.PI/2 + theta), (1.5 * Math.PI + theta), false);
		context.lineTo(mouseX + radius * Math.sin(theta) + length * Math.cos(theta), 
		  mouseY - radius * Math.cos(theta) + length * Math.sin(theta));
		context.arc(mouseX + length * Math.cos(theta), mouseY + length * Math.sin(theta), 
		  radius, (1.5 * Math.PI + theta), (Math.PI/2 + theta), false);
		context.closePath();
	  }
	  context.fillStrokeShape(this);
	},
	stroke: 'black',
	strokeWidth: 2,
	listening: false
  });
  layer.add(roundRect);
  layer.draw();
  return roundRect;
}

function CheckSolution(){
  var cell1 = parseInt(Math.floor((y1 - (cHeight * 0.02)) / (cHeight * 0.1)) + '' + Math.floor((x1 - (cWidth * 0.02)) / (cWidth * 0.1)));
  var cell2 = parseInt(Math.floor((y2 - (cHeight * 0.02)) / (cHeight * 0.1)) + '' + Math.floor((x2 - (cWidth * 0.02)) / (cWidth * 0.1)));
  var currentCell;
  
  for(var i = 0; i < check.length; i++){
    currentCell = check[i].split(',');
	if((cell1 == currentCell[0] && cell2 == currentCell[1]) || (cell1 == currentCell[1] && cell2 == currentCell[0])){
	  $('#mn' + i).addClass('correct');
	  return false;
	}
  }
  return true;
}