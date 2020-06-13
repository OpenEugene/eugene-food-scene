using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPro.BlazorSize;
using EugeneFoodScene.Client.Services;
using Microsoft.AspNetCore.Components;

namespace EugeneFoodScene.Client.Shared
{
    public abstract class ResizableCachedComponent : ComponentBase, IDisposable
    {
        protected bool IsMedium = false;
        [Inject] protected ClientCache Cache { get; set; }

        [Inject] private ResizeListener Listener { get; set; }


        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Cache.CacheUpdated += OnCacheUpdated;
                Listener.OnResized += WindowResized;
            }

        }

        async void WindowResized(object _, BrowserWindowSize window)
        {

            // /// Medium devices (tablets, less than 992px)
            /// @media(max-width: 991.98px) { ... } link to all fo them: https://github.com/EdCharbeneau/BlazorSize
            IsMedium = await Listener.MatchMedia(Breakpoints.MediumDown);

            // We're outside of the component's lifecycle, be sure to let it know it has to re-render.
            StateHasChanged();
        }

        public void Dispose()
        {
            Cache.CacheUpdated -= OnCacheUpdated;
        }

        private void OnCacheUpdated(object sender, EventArgs e)
        {
            this.StateHasChanged();
        }
    }
}
