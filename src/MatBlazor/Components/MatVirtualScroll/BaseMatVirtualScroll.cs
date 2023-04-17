using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatBlazor
{
    /// <summary>
    /// The VirtualScroll for Blazor displays large lists of elements performantly by only rendering the items that fit on-screen.
    /// Loading hundreds of elements can be slow in any browser; virtual scrolling enables a performant way to simulate all items being rendered by making the height of the container element the same as the height of total number of elements to be rendered, and then only rendering the items in view.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class BaseMatVirtualScroll<TItem> : BaseMatDomComponent, IMatVirtualScroll
    {
        [Parameter]
        public int ItemHeight { get; set; } = 50;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
        
        [Parameter]
        public RenderFragment<TItem> ItemTemplate { get; set; }

        [Parameter]
        public IEnumerable<TItem> Items { get; set; }

        public MatDotNetObjectReference<MatVirtualScrollJsHelper> JsHelperReference { get; set; }


        public BaseMatVirtualScroll()
        {
            ClassMapper.Add("﻿mat-virtual-scroll");

            CallAfterRender(async () =>
            {
                if (!Disabled)
                {
                    JsHelperReference =
                        new MatDotNetObjectReference<MatVirtualScrollJsHelper>(new MatVirtualScrollJsHelper(this));
                    var scrollView = await Js.InvokeAsync<MatVirtualScrollView>("matBlazor.matVirtualScroll.init", Ref,
                        JsHelperReference.Reference);
                    this.SetScrollView(scrollView);
                }
            });
        }

        public void VirtualScrollingSetView(MatVirtualScrollView scrollView)
        {
            this.SetScrollView(scrollView);
        }


        public override void Dispose()
        {
            base.Dispose();
            // todo call js to dispose events (window resize)
            JsHelperReference?.Dispose();
        }


        protected string GetContentStyle()
        {
            if (Disabled)
            {
                return $"height: {Items.Count() * ItemHeight}px;";
            }

            return
                $"height: {ScrollViewResult.Height}px; padding-top: {(ScrollViewResult.SkipItems * ItemHeight)}px;";
        }

        protected IEnumerable<TItem> GetContentItems()
        {
            if (Disabled)
            {
                return Items;
            }

            return
                Items.Skip(ScrollViewResult.SkipItems).Take(ScrollViewResult.TakeItems);
        }

        private void SetScrollView(MatVirtualScrollView scrollView)
        {
            this.ScrollView = scrollView;
            this.ScrollViewResult = new MatVirtualScrollViewResult
            {
                Height = Items.Count() * ItemHeight,
                SkipItems = scrollView.ScrollTop / this.ItemHeight
            };
            this.ScrollViewResult.TakeItems =
                (int) Math.Ceiling((double) (scrollView.ScrollTop + scrollView.ClientHeight) / (double) ItemHeight) -
                this.ScrollViewResult.SkipItems;
//            Console.WriteLine(ScrollViewResult.SkipItems + " " + ScrollViewResult.TakeItems);
            this.StateHasChanged();
        }

        public MatVirtualScrollViewResult ScrollViewResult { get; set; }

        public MatVirtualScrollView ScrollView { get; set; }
    }
}