﻿using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace MatBlazor
{
    /// <summary>
    /// Material Design Checkboxes for Blazor, allow the user to select multiple options from a set. 
    /// </summary>
    /// <typeparam name="TValue">bool, bool?</typeparam>
    public class BaseMatCheckboxInternal<TValue> : BaseMatInputElementComponent<TValue>
    {
        protected ElementReference ComponentRef { get; set; }

        public BaseMatCheckboxInternal()
        {
            ClassMapper
                .Add("mat-checkbox")
                .Add("mdc-form-field")
                .If("mdc-checkbox--primary", () => Color == MatComponentColor.Primary)
                .If("mdc-checkbox--secondary", () => Color == MatComponentColor.Secondary)
                .If("mdc-checkbox--success", () => Color == MatComponentColor.Success)
                .If("mdc-checkbox--warning", () => Color == MatComponentColor.Warning)
                .If("mdc-checkbox--danger", () => Color == MatComponentColor.Danger)
                .If("mdc-checkbox--info", () => Color == MatComponentColor.Info);

            CallAfterRender(async () =>
            {
                await JsInvokeVoidAsync("matBlazor.matCheckbox.init", Ref, ComponentRef);
            });
        }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Label { get; set; }


        [Parameter]
        public bool Indeterminate { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string InputValue { get; set; }

        [Parameter]
        public MatComponentColor Color { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!Indeterminate)
            {
                return;
            }
            CallAfterRender(async () =>
            {
                await JsInvokeVoidAsync("matBlazor.matCheckbox.setIndeterminate", Ref, CurrentValue == null);
            });
        }

        protected void ChangeHandler(ChangeEventArgs e)
        {
            var newValue = (bool)e.Value;
            if (Indeterminate)
            {
                CurrentValue = CurrentValue switch
                {
                    true => SwitchT.FromBoolNull(false, Indeterminate),
                    false => SwitchT.FromBoolNull(null, Indeterminate),
                    _ => SwitchT.FromBoolNull(true, Indeterminate)
                };
            }
            else
            {
                CurrentValue = SwitchT.FromBoolNull(newValue, Indeterminate);
            }
        }
    }
}