using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatBlazor
{
    public partial class MatChipSet 
    {
        public MatChipSet()
        {
            ClassMapper
                .Add("mdc-chip-set")
                .If("mdc-chip-set--choice", () => this.Choice)
                .If("mdc-chip-set--filter", () => this.Filter);
        }

        protected async override Task OnFirstAfterRenderAsync()
        {
            await base.OnFirstAfterRenderAsync();
            await JsInvokeAsync<object>("matBlazor.matChipSet.init", Ref);
        }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Allows single selection from a set of options. If combined with Filter the selected value can be unselected.
        /// </summary>
        [Parameter]
        public bool Choice { get; set; }

        /// <summary>
        ///  Enables multiple-choice selection from the set of chips. Chips must be "Checkable" for this to work.
        /// </summary>
        [Parameter]
        public bool Filter { get; set; }

        [Parameter]
        public MatChip SelectedChip
        {
            get { return _chips.OfType<MatChip>().Where(x => x.IsSelected).FirstOrDefault(); }
            set
            {
                if (value == null)
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = false;
                    }
                }
                else
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = (chip == value);
                    }
                }
                this.InvokeAsync(StateHasChanged);
            }
        }

        [Parameter]
        public EventCallback<MatChip> SelectedChipChanged { get; set; }

        [Parameter]
        public MatChip[] SelectedChips
        {
            get { return _chips.OfType<MatChip>().Where(x => x.IsSelected).ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                {
                    foreach (var chip in _chips)
                    {
                        chip.IsSelected = false;
                    }
                }
                else
                {
                    var selected = new HashSet<string>(value.Select(x => x.Label));
                    foreach (var chip in _chips)
                        chip.IsSelected = selected.Contains(chip.Label);
                }
                this.InvokeAsync(StateHasChanged);
            }
        }

        [Parameter]
        public EventCallback<MatChip[]> SelectedChipsChanged { get; set; }

        private readonly HashSet<MatChip> _chips = new HashSet<MatChip>();

        public void RegisterChip(MatChip chip)
        {
            _chips.Add(chip);
        }

        public async Task UnregisterChip(MatChip chip)
        {
            if (chip == null)
            {
                return;
            }
            await NotifySelection(); // <-- removing a selected chip updates 
        }

        public async Task HandleChipClicked(MatChip chip)
        {
            if (Filter)
            {
                chip.IsSelected = !chip.IsSelected;
            }
            else if (Choice)
            {
                chip.IsSelected = true; 
            }
            await NotifySelection();
        }

        private async Task NotifySelection()
        {
            await InvokeAsync(StateHasChanged);
            await SelectedChipChanged.InvokeAsync(SelectedChip);
            await SelectedChipsChanged.InvokeAsync(SelectedChips);
        }

        public async Task HandleChipSelected(MatChip chip)
        {
            if (!Choice)
            {
                return;
            }
            foreach (var ch in _chips)
            {
                if (ch != chip)
                {
                    ch.IsSelected = false; // <-- exclusively select the one chip only, thus all others must be deselected
                }
            }
            await NotifySelection();
        }

    }
}