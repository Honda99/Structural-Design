window.Canvas = {
    
    font: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.font = value[0];
    },
    textAlign: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.textAlign = value[0];
    },
    textBaseline: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.textBaseline = value[0];
    },
    textDirection: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.direction = value[0];
    },
    fillStyle: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.fillStyle = value[0];
    },
    strokeStyle: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.strokeStyle = value[0];
        
    },
  
    fillRect: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.fillRect(value[0], value[1], value[2],value[3]);
    },
    clearRect: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.clearRect(value[0], value[1], value[2], value[3]);
    },
    strokeRect: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.strokeRect(value[0], value[1], value[2], value[3]);
    },
    fillText: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.fillText(value[0], value[1],value[2]);
    },
    strokeText: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.strokeText(value[0], value[1], value[2]);

    },
    measureText: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
      return  ctx.measureText(value[0]).width
    },
    lineWidth: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.lineWidth  = value[0];
    },
    lineCap: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.lineCap = value[0];
    },
    lineJoin: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.lineJoin = value[0];
    },
    getLineDash: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        return ctx.getLineDash();
    },
    setLineDash: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.setLineDash([value[0], value[1]]);
    },
    lineDashOffset: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.lineDashOffset = value[0];
    },
    shadowBlur: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.shadowBlur = value[0];
      
    },
    shadowColor: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.shadowColor = value[0];
    },
    shadowOffsetX: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.shadowOffsetX = value[0];
    },
    shadowOffsetY: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.shadowOffsetY = value[0];
    },
    beginPath: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.beginPath();
    },
    closePath: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.closePath();
    },
    moveTo: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.moveTo(value[0], value[1]);
    },
    lineTo: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.lineTo(value[0], value[1]);
    },
    bezierCurveTo: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.bezierCurveTo(value[0], value[1], value[2], value[3], value[4], value[5]);
    },
    quadraticCurveTo: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.quadraticCurveTo(value[0], value[1], value[2], value[3]);
    },
    arc: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        if (value.length === 5) {
            ctx.arc(value[0], value[1], value[2], value[3], value[4]);
        }
        else {
            ctx.arc(value[0], value[1], value[2], value[3], value[4],true);
        }
       
    },
    arcTo: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.arcTo(value[0], value[1], value[2], value[3], value[4]);
    },
    rect: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.rect(value[0], value[1], value[2], value[3]);
    },
    fill: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.fill();

    },
    stroke: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.stroke();
    },
    drawFocusIfNeeded: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        var elem = document.getElementById(value[0]);
        elem.focus();
        ctx.drawFocusIfNeeded(elem);
    },
    scrollPathIntoView: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.scrollPathIntoView();
    },
    clip: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.clip();
    },
    isPointInPath: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.isPointInPath(value[0], value[1]);
      
    },
    isPointInStroke: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.isPointInStroke(value[0], value[1]);
    },
    rotate: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.rotate(value[0]);
    },
    scale: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.sclae(value[0], vlaue[1]);
    },
    translate: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.translate(value[0], value[1]);
    },
    transform: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.transform(value[0], vlaue[1], value[2], vlaue[3], value[4], value[5]);
    },
    setTransform: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.setTransform(value[0], vlaue[1], value[2], vlaue[3], value[4], value[5]);
    },
    globalAlpha: function (canva, value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.globalAlpha = value[0];
    },
    save: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.save();
    },
    restore: function (canva) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.restore();
    },
    miterLimit: function (canva,value) {
        var canv = canva;
        var ctx = canv.getContext("2d");
        ctx.miterLimit = value[0];
    },
};
