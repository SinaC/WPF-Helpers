using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SampleWPF.Core;
using SampleWPF.Core.Commands;
using SampleWPF.Core.Interfaces;
using SampleWPF.Core.MVVM;

namespace SampleWPF.ViewModels.Popups
{
    public class QuestionPopupViewModel : ViewModelBase
    {
        public IPopupService PopupService { get; private set; }

        private string _question;
        public string Question
        {
            get { return _question; }
            set { Set(() => Question, ref _question, value); }
        }

        private List<QuestionPopupAnswerItem> _answerItems;
        public List<QuestionPopupAnswerItem> AnswerItems
        {
            get { return _answerItems; }
            set { Set(() => AnswerItems, ref _answerItems, value); }
        }

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                _clickCommand = _clickCommand ?? new GenericRelayCommand<QuestionPopupAnswerItem>(Click);
                return _clickCommand;
            }
        }

        public QuestionPopupViewModel(IPopupService popupService)
        {
            PopupService = popupService;
        }

        public void Initialize(string question, params ActionButton[] actionButtons)
        {
            Question = question;
            AnswerItems = (actionButtons ?? Enumerable.Empty<ActionButton>())
                .OrderBy(x => x.Order)
                .Select(x => new QuestionPopupAnswerItem
                    {
                        Caption = x.Caption,
                        ClickCallback = x.ClickCallback,
                        CloseOnClick = x.CloseOnClick
                    }).ToList();
        }

        private void Click(QuestionPopupAnswerItem answer)
        {
            if (answer != null)
            {
                if (answer.CloseOnClick)
                    PopupService.Close(this);
                if (answer.ClickCallback != null)
                    answer.ClickCallback();
            }
        }
    }

    public class QuestionPopupViewModelDesignData : QuestionPopupViewModel
    {
        public QuestionPopupViewModelDesignData() : base(null)
        {
            Question = "How much wood could a woodchuck chuck if a woodchuck could chuck wood ?";
            AnswerItems = new List<QuestionPopupAnswerItem>
                {
                    new QuestionPopupAnswerItem
                        {
                            Caption = "A lot"
                        },
                    new QuestionPopupAnswerItem
                        {
                            Caption = "42"
                        },
                    new QuestionPopupAnswerItem
                        {
                            Caption = "I dont' care"
                        },
                };
        }
    }
}
