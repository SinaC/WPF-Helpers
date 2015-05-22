using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ModalPopupOverlay.ViewModels
{
    public class QuestionPopupViewModel : ViewModelBase
    {
        private string _question;
        public string Question
        {
            get { return _question; }
            set
            {
                if (_question != value)
                {
                    _question = value;
                    OnPropertyChanged("Question");
                }
            }
        }

        private List<QuestionPopupAnswerItem> _answerItems;
        public List<QuestionPopupAnswerItem> AnswerItems
        {
            get { return _answerItems; }
            set
            {
                if (_answerItems != value)
                {
                    _answerItems = value;
                    OnPropertyChanged("AnswerItems");
                }
            }
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

        public void Initialize(string question, params ActionButton[] actionButtons)
        {
            Question = question;
            AnswerItems = (actionButtons ?? Enumerable.Empty<ActionButton>())
                .OrderBy(x => x.Order)
                .Select(x => new QuestionPopupAnswerItem
                    {
                        Caption = x.Caption,
                        ClickCallback = x.ClickCallback
                    }).ToList();
        }

        private void Click(QuestionPopupAnswerItem answer)
        {
            if (answer != null && answer.ClickCallback != null)
                answer.ClickCallback();
        }
    }

    public class QuestionPopupViewModelDesignData : QuestionPopupViewModel
    {
        public QuestionPopupViewModelDesignData()
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
