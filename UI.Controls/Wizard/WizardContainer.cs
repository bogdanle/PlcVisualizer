using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI.Controls.Wizard
{
    [TemplatePart(Name = "NextButton", Type = typeof(Button))]
    [TemplatePart(Name = "BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "FinishButton", Type = typeof(Button))]
    [TemplatePart(Name = "CancelButton", Type = typeof(Button))]
    [TemplatePart(Name = "ProgressLadder", Type = typeof(WizardProgressLadder))]
    [TemplatePart(Name = "PageContainer", Type = typeof(ContentPresenter))]
    public class WizardContainer : ContentControl, INotifyPropertyChanged
    {
        private VisualBrush _prevContentBrush;
        private Border _contentPlaceholder;
        private Canvas _prevContentPlaceholder;
        private bool _finishSequencePaused;
        private UserControl _currentPage;
        private int _iteration = 1;
        private string _transitionName = "MoveNext";
        private bool _eventsSubscribed;
        private Button _nextButton;
        private Button _backButton;
        private Button _finishButton;
        private Button _cancelButton;
        private ContentPresenter _pageContainer;

        public WizardContainer()
        {
            DefaultStyleKey = typeof(WizardContainer);
            DataContext = this;

            PageManager.CurrentPageChanged += PageManager_CurrentPageChanged;
            PageManager.CurrentPageStatusChanged += PageManager_CurrentPageStatusChanged;
            PageManager.PageCollectionChanged += PageManager_PageCollectionChanged;
            PageManager.CurrentPagePropertyChanged += PageManager_PagePropertyChanged;
        }

        public event EventHandler<CancelEventArgs> WizardClosed;

        public event PropertyChangedEventHandler PropertyChanged;

        public string WizardPageTitle { get; set; }

        public string PrevWizardPageTitle { get; set; }

        public string ProgressLadderTitle { get; set; } = "Progress Ladder Title";

        public UserControl CurrentPage
        {
            get => _currentPage;

            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }

        public IWizardPageManager PageManager { get; set; } = new WizardPageManager();

        public Visibility FinishButtonVisibility => PageManager.CanGoForward() ? Visibility.Collapsed : Visibility.Visible;

        public bool IsBackEnabled => _backButton != null && PageManager.CanGoBack();

        public bool IsNextEnabled => PageManager.CanGoForward() && PageManager.CurrentPageStatus != WizardPageStatus.Invalid;

        public bool IsFinishEnabled => PageManager.CurrentPageStatus != WizardPageStatus.Invalid;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _nextButton = GetTemplateChild("NextButton") as Button;
            _backButton = GetTemplateChild("BackButton") as Button;
            _finishButton = GetTemplateChild("FinishButton") as Button;
            _cancelButton = GetTemplateChild("CancelButton") as Button;
            _pageContainer = GetTemplateChild("PageContainer") as ContentPresenter;

            _contentPlaceholder = (Border)GetTemplateChild("contentPlaceholder");
            _prevContentPlaceholder = (Canvas)GetTemplateChild("prevContentPlaceholder");

            SubscribeToEvents();
        }

        public void HideBackAndCancelButtons()
        {
            _backButton.Visibility = Visibility.Collapsed;
            _cancelButton.Visibility = Visibility.Collapsed;
        }

        public void ChangeNextButtonCaption(string newCaption)
        {
            if (newCaption == "Next")
            {
                var content = new StackPanel() { Orientation = Orientation.Horizontal };
                content.Children.Add(new TextBlock() { Text = "Next", VerticalAlignment = VerticalAlignment.Center });
                var image = new Image() { Margin = new Thickness(5, 1, 0, 0), Stretch = Stretch.Uniform, Width = 16, VerticalAlignment = VerticalAlignment.Center };
                image.SetResourceReference(Image.SourceProperty, "RightArrowIcon");
                content.Children.Add(image);

                _nextButton.Content = content;
            }
            else
            {
                _nextButton.Content = newCaption;
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string info = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void PauseFinishSequence()
        {
            _finishSequencePaused = true;
        }

        private void ResumeFinishSequence()
        {
            _finishSequencePaused = false;
            ExecuteFinishProcess();
        }

        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            OnNextClicked();
        }

        private void BackButton_Clicked(object sender, RoutedEventArgs e)
        {
            OnBackClicked();
        }

        private void FinishButton_Clicked(object sender, RoutedEventArgs e)
        {
            OnFinishClicked();
        }

        private void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            OnCancelClicked();
        }

        private void PageManager_CurrentPageChanged(object sender, WizardPageChangedEventArgs e)
        {
            OnCurrentPageChanged(e);
        }

        private void PageManager_CurrentPageStatusChanged(object sender, WizardPageStatusChangedEventArgs e)
        {
            OnCurrentPageStatusChanged(e);
        }

        private void PageManager_PageCollectionChanged(object sender, EventArgs e)
        {
            OnPageCollectionChanged(e);
        }

        private void PageManager_PagePropertyChanged(object sender, WizardPagePropertyChangedEventArgs e)
        {
            OnCurrentPagePropertyChanged(e);
        }

        private void OnBackClicked()
        {
            _iteration = _iteration == 1 ? 0 : 1;
            _transitionName = "MoveBack" + _iteration;
            PageManager.GoToPreviousPage();
        }

        private void OnNextClicked()
        {
            _iteration = _iteration == 1 ? 0 : 1;
            _transitionName = "MoveNext" + _iteration;
            PageManager.GoToNextPage();
        }

        private void OnBeforeFinish()
        {
            PageManager.SetPageStatus(PageManager.CurrentPage.PageName, WizardPageStatus.Invalid);
        }

        private void OnFinishClicked()
        {
            OnBeforeFinish();

            if (!_finishSequencePaused)
            {
                ExecuteFinishProcess();
            }
        }

        private void OnCancelClicked()
        {
            PageManager.Cancel();

            WizardClosed?.Invoke(this, new CancelEventArgs { Cancel = true });
        }

        private void OnCurrentPageChanged(WizardPageChangedEventArgs e)
        {                                
            if (CurrentPage != null && _prevContentBrush == null)
            {
                FrameworkElement content = CurrentPage;
                if (content != null)
                {
                    _prevContentBrush = new VisualBrush(content)
                    {
                        Viewbox = new Rect(0, 0, content.ActualWidth, content.ActualHeight), ViewboxUnits = BrushMappingMode.Absolute, Stretch = Stretch.None
                    };

                    _prevContentPlaceholder.Background = _prevContentBrush;
                    _prevContentPlaceholder.Opacity = 1;
                    _prevContentBrush = null;
                }
            }
            else
            {
                _prevContentBrush = null;
            }

            CurrentPage = e.NewPage.PageView;
            PrevWizardPageTitle = WizardPageTitle;
            WizardPageTitle = e.NewPage.PageTitle;

            RaisePropertyChanged(nameof(IsNextEnabled));
            RaisePropertyChanged(nameof(IsBackEnabled));
            RaisePropertyChanged(nameof(IsFinishEnabled));
            RaisePropertyChanged(nameof(FinishButtonVisibility));
            RaisePropertyChanged(nameof(CurrentPage));
            RaisePropertyChanged(nameof(ProgressLadderTitle));
            RaisePropertyChanged(nameof(WizardPageTitle));
            RaisePropertyChanged(nameof(PrevWizardPageTitle));

            VisualStateManager.GoToState(this, _transitionName, true);
        }

        private void OnCurrentPageStatusChanged(WizardPageStatusChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsNextEnabled));
            RaisePropertyChanged(nameof(IsFinishEnabled));
        }

        private void OnCurrentPagePropertyChanged(WizardPagePropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsBackEnabled));
        }

        private void OnPageCollectionChanged(EventArgs e)
        {
        }

        private void ExecuteFinishProcess()
        {
            WizardClosed?.Invoke(this, new CancelEventArgs { Cancel = false });
            PageManager.Finish();
        }

        private void SubscribeToEvents()
        {
            if (!_eventsSubscribed)
            {
                _eventsSubscribed = true;

                _nextButton.Click += OnNextClicked;
                _backButton.Click += BackButton_Clicked;
                _finishButton.Click += FinishButton_Clicked;
                _cancelButton.Click += CancelButton_Clicked;
            }
        }
    }
}
