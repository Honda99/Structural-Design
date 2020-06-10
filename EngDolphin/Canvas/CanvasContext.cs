using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Canvas
{
    public class CanvasContext:RenderingContext
    {
        #region Constants
        //private const string CONTEXT_NAME = "Canvas2d";
        private const string FILL_STYLE_PROPERTY = "fillStyle";
        private const string STROKE_STYLE_PROPERTY = "strokeStyle";
        private const string FILL_RECT_METHOD = "fillRect";
        private const string CLEAR_RECT_METHOD = "clearRect";
        private const string STROKE_RECT_METHOD = "strokeRect";
        private const string FILL_TEXT_METHOD = "fillText";
        private const string STROKE_TEXT_METHOD = "strokeText";
        private const string MEASURE_TEXT_METHOD = "measureText";
        private const string LINE_WIDTH_PROPERTY = "lineWidth";
        private const string LINE_CAP_PROPERTY = "lineCap";
        private const string LINE_JOIN_PROPERTY = "lineJoin";
        private const string MITER_LIMIT_PROPERTY = "miterLimit";
        private const string GET_LINE_DASH_METHOD = "getLineDash";
        private const string SET_LINE_DASH_METHOD = "setLineDash";
        private const string LINE_DASH_OFFSET_PROPERTY = "lineDashOffset";
        private const string SHADOW_BLUR_PROPERTY = "shadowBlur";
        private const string SHADOW_COLOR_PROPERTY = "shadowColor";
        private const string SHADOW_OFFSET_X_PROPERTY = "shadowOffsetX";
        private const string SHADOW_OFFSET_Y_PROPERTY = "shadowOffsetY";
        private const string BEGIN_PATH_METHOD = "beginPath";
        private const string CLOSE_PATH_METHOD = "closePath";
        private const string MOVE_TO_METHOD = "moveTo";
        private const string LINE_TO_METHOD = "lineTo";
        private const string BEZIER_CURVE_TO_METHOD = "bezierCurveTo";
        private const string QUADRATIC_CURVE_TO_METHOD = "quadraticCurveTo";
        private const string ARC_METHOD = "arc";
        private const string ARC_TO_METHOD = "arcTo";
        private const string RECT_METHOD = "rect";
        private const string FILL_METHOD = "fill";
        private const string STROKE_METHOD = "stroke";
        private const string DRAW_FOCUS_IF_NEEDED_METHOD = "drawFocusIfNeeded";
        private const string SCROLL_PATH_INTO_VIEW_METHOD = "scrollPathIntoView";
        private const string CLIP_METHOD = "clip";
        private const string IS_POINT_IN_PATH_METHOD = "isPointInPath";
        private const string IS_POINT_IN_STROKE_METHOD = "isPointInStroke";
        private const string ROTATE_METHOD = "rotate";
        private const string SCALE_METHOD = "scale";
        private const string TRANSLATE_METHOD = "translate";
        private const string TRANSFORM_METHOD = "transform";
        private const string SET_TRANSFORM_METHOD = "setTransform";
        private const string GLOBAL_ALPHA_PROPERTY = "globalAlpha";
        private const string SAVE_METHOD = "save";
        private const string RESTORE_METHOD = "restore";
        private const string TEXTALIGN_PROPERTY= "textAlign";
        private const string TEXTDIRECTION_PROPERTY = "textDirection";
        private const string TEXTBASELINE_PROPERTY = "textBaseline";
        private const string FONT_PROPERTY = "font";
        #endregion
        #region Properties
        public TextAlign TextAlign { get; private set; }

        public TextDirection Direction { get; private set; }

        public TextBaseline TextBaseline { get; private set; }

        public float LineWidth { get; private set; } = 1.0f;

        public LineCap LineCap { get; private set; }

        public LineJoin LineJoin { get; private set; }
        public string FillStyle { get; private set; } = "#000";

        public string StrokeStyle { get; private set; } = "#000";

        public string Font { get; private set; } = "10px sans-serif";
        public float MiterLimit { get; private set; } = 10;

        public float LineDashOffset { get; private set; }

        public float ShadowBlur { get; private set; }

        public string ShadowColor { get; private set; } = "black";

        public float ShadowOffsetX { get; private set; }

        public float ShadowOffsetY { get; private set; }

        public float GlobalAlpha { get; private set; } = 1.0f;
        public ElementReference canRef { get; set; }
        public IJSRuntime _JsRuntime { get; set; }
        #endregion Properties
        #region constructor
         public CanvasContext(ElementReference canvas,IJSRuntime js):base(canvas,js)
        {
            
        }
       

        //public static CanvasContext CreateAsync(ElementRef canv,IJSRuntime js)
        //{
        //    var ret = new CanvasContext(canv,js);
        //    return ret.InitializeAsync().GetAwaiter().GetResult() as CanvasContext;
        //}
        #endregion
        #region Property Setters

        public void SetFillStyle(string value)
        {
            this.FillStyle = value;
            this.CallMethod<object>(FILL_STYLE_PROPERTY, value);

        }
       

        public void SetStrokeStyle(string value)
        {
            this.StrokeStyle = value;
            this.CallMethod<object>(STROKE_STYLE_PROPERTY, value);
        }

        public void SetFont(string value)
        {
            this.Font = value;
           this.CallMethod<object>(FONT_PROPERTY, value);
        }
        public void SetMiterLimit(float value)
        {
            this.MiterLimit = value;
          this.CallMethod<object>(MITER_LIMIT_PROPERTY, value);
        }

        public void SetTextAlign(TextAlign value)
        {
            this.TextAlign = value;
           this.CallMethod<object>(TEXTALIGN_PROPERTY, value.ToString().ToLowerInvariant());

        }

        public void SetTextDirection(TextDirection value)
        {
            this.Direction = value;
          this.CallMethod<object>(TEXTDIRECTION_PROPERTY, value.ToString().ToLowerInvariant());
        }

        public void SetTextBaseline(TextBaseline value)
        {
            this.TextBaseline = value;
            this.CallMethod<object>(TEXTBASELINE_PROPERTY, value.ToString().ToLowerInvariant());
        }

        public void SetLineWidth(float value)
        {
            this.LineWidth = value;
           this.CallMethod<object>(LINE_WIDTH_PROPERTY, value);
        }
        public void SetGlobalAlpha(float value)
        {
            this.GlobalAlpha = value;
            this.CallMethod<object>(GLOBAL_ALPHA_PROPERTY, value);
        }

        public void SetLineCap(LineCap value)
        {
            this.LineCap = value;
          this.CallMethod<object>(LINE_CAP_PROPERTY, value.ToString().ToLowerInvariant());
        }

        public void SetLineJoin(LineJoin value)
        {
            this.LineJoin = value;
       this.CallMethod<object>(LINE_JOIN_PROPERTY, value.ToString().ToLowerInvariant());
        }

      

        public void SetLineDashOffset(float value)
        {
            this.LineDashOffset = value;
          this.CallMethod<object>(LINE_DASH_OFFSET_PROPERTY, value);
        }

        public void SetShadowBlur(float value)
        {
            this.ShadowBlur = value;
            this.CallMethod<object>(SHADOW_BLUR_PROPERTY, value);
        }

        public void SetShadowColor(string value)
        {
            this.ShadowColor = value;
           this.CallMethod<object>(SHADOW_COLOR_PROPERTY, value);
        }

        public void SetShadowOffsetX(float value)
        {
            this.ShadowOffsetX = value;
            this.CallMethod<object>(SHADOW_OFFSET_X_PROPERTY, value);
        }

        public void SetShadowOffset(float value)
        {
            this.ShadowOffsetY = value;
            this.CallMethod<object>(SHADOW_OFFSET_Y_PROPERTY, value);
        }

      

        #endregion Property Setters

        #region Methods
        public void FillRect(double x, double y, double width, double height) => this.CallMethod<object>(FILL_RECT_METHOD, x, y, width, height);

        public void ClearRect(double x, double y, double width, double height) =>this.CallMethod<object>(CLEAR_RECT_METHOD, x, y, width, height);

        public void StrokeRect(double x, double y, double width, double height) =>this.CallMethod<object>(STROKE_RECT_METHOD, x, y, width, height);

        public void FillText(string text, double x, double y, double? maxWidth = null) => this.CallMethod<object>(FILL_TEXT_METHOD, maxWidth.HasValue ? new object[] { text, x, y, maxWidth.Value } : new object[] { text, x, y });

        public void StrokeText(string text, double x, double y, double? maxWidth = null) => this.CallMethod<object>(STROKE_TEXT_METHOD, maxWidth.HasValue ? new object[] { text, x, y, maxWidth.Value } : new object[] { text, x, y });

        //public TextMetrics MeasureText(string text) => this.CallMethod<TextMetrics>(MEASURE_TEXT_METHOD, text);

        //public  float[]GetLineDash() =>this.CallMethod<float[]>(GET_LINE_DASH_METHOD);
        //public  float MeasureText(string value) =>this.CallMethod<float>(MEASURE_TEXT_METHOD,value);

        public void SetLineDash(float[] segments) => this.CallMethod<object>(SET_LINE_DASH_METHOD, segments);

        public void BeginPath() => this.CallMethod<object>(BEGIN_PATH_METHOD);

        public void ClosePath() => this.CallMethod<object>(CLOSE_PATH_METHOD);

        public void MoveTo(double x, double y) => this.CallMethod<object>(MOVE_TO_METHOD, x, y);

        public void LineTo(double x, double y) => this.CallMethod<object>(LINE_TO_METHOD, x, y);

        public void BezierCurveTo(double cp1X, double cp1Y, double cp2X, double cp2Y, double x, double y) => this.CallMethod<object>(BEZIER_CURVE_TO_METHOD, cp1X, cp1Y, cp2X, cp2Y, x, y);

        public void QuadraticCurveTo(double cpx, double cpy, double x, double y) => this.CallMethod<object>(QUADRATIC_CURVE_TO_METHOD, cpx, cpy, x, y);

        public void Arc(double x, double y, double radius, double startAngle, double endAngle, bool? anticlockwise = null) => this.CallMethod<object>(ARC_METHOD, anticlockwise.HasValue ? new object[] { x, y, radius, startAngle, endAngle, anticlockwise.Value } : new object[] { x, y, radius, startAngle, endAngle });

        public void ArcTo(double x1, double y1, double x2, double y2, double radius) => this.CallMethod<object>(ARC_TO_METHOD, x1, y1, x2, y2, radius);

        public void Rect(double x, double y, double width, double height) =>this.CallMethod<object>(RECT_METHOD, x, y, width, height);

        public void Fill() => this.CallMethod<object>(FILL_METHOD);

        public void Stroke() => this.CallMethod<object>(STROKE_METHOD);

        //public void DrawFocusIfNeeded(ElementRef elementReference) => this.CallMethod<object>(DRAW_FOCUS_IF_NEEDED_METHOD, elementReference);

        public void ScrollPathIntoView() => this.CallMethod<object>(SCROLL_PATH_INTO_VIEW_METHOD);

        public void Clip() => this.CallMethod<object>(CLIP_METHOD);

        //public  bool IsPointInPath(double x, double y) => this.CallMethod<bool>(IS_POINT_IN_PATH_METHOD, x, y);

        //public bool IsPointInStroke(double x, double y) => this.CallMethod<bool>(IS_POINT_IN_STROKE_METHOD, x, y);

        public void Rotate(float angle) =>this.CallMethod<object>(ROTATE_METHOD, angle);
        public void DrawFocusIfNeeded(string elmId) =>this.CallMethod<object>(DRAW_FOCUS_IF_NEEDED_METHOD, elmId);

        public void Scale(double x, double y) => this.CallMethod<object>(SCALE_METHOD, x, y);

        public void Translate(double x, double y) =>this.CallMethod<object>(TRANSLATE_METHOD, x, y);

        public void Transform(double m11, double m12, double m21, double m22, double dx, double dy) => this.CallMethod<object>(TRANSFORM_METHOD, m11, m12, m21, m22, dx, dy);

        public void SetTransform(double m11, double m12, double m21, double m22, double dx, double dy) => this.CallMethod<object>(SET_TRANSFORM_METHOD, m11, m12, m21, m22, dx, dy);

        public void Save() => this.CallMethod<object>(SAVE_METHOD);

        public void Restore() => this.CallMethod<object>(RESTORE_METHOD);

        #endregion Methods

    }
}
