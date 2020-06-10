using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EngDolphin.Canvas
{
    public class RenderingContext
    {
        private readonly Microsoft.JSInterop.IJSRuntime _jsRuntime;
        public ElementReference Canvas { get; }
        private string Context = "Canvas";
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
      
        public  RenderingContext(ElementReference refCanv,IJSRuntime js)
        {
            Canvas = refCanv;
            _jsRuntime = js;
        }
//#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously; Reason: extension point for subclasses, which may do asynchronous work
//        protected virtual async Task ExtendedInitializeAsync() { }
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

//        internal async Task<RenderingContext> InitializeAsync()
//        {
//            await this._semaphoreSlim.WaitAsync();
//            if (!this._initialized)
//            {
//                await this._jsRuntime.InvokeAsync<object>($"{this.Context}.{StartCanvas}");
//                await this.ExtendedInitializeAsync();
//                this._initialized = true;
//            }
//            this._semaphoreSlim.Release();
//            return this;
//        }

        protected  void CallMethod<T>(string method)
        {
          var ob=  this._jsRuntime.InvokeAsync<T>($"{Context}.{method}", this.Canvas);
            
        }

        protected async Task<T> CallMethodAsync<T>(string method)
        {
            return await this._jsRuntime.InvokeAsync<T>($"{Context}.{method}", this.Canvas);
        }

        protected void CallMethod<T>(string method, params object[] value)
        {
            var ob= this._jsRuntime.InvokeAsync<T>($"{Context}.{method}", this.Canvas, value);
            
        }
        protected async Task<T> CallMethodAsync<T>(string method, params object[] value)
        {
            return await this._jsRuntime.InvokeAsync<T>($"{Context}.{method}", this.Canvas , value);
        }

    }
}
