using DevExpress.Mvvm;
using System.Collections.ObjectModel;

namespace LiftLogicPro.ViewModels.Base
{
    public abstract class CrudViewModelBase<T> : ViewModelBase where T : class
    {
        public virtual ObservableCollection<T> Items { get; set; }
        public virtual T SelectedItem { get; set; }
        public virtual bool IsEditing { get; set; }
        public virtual bool IsLoading { get; set; }

        public DelegateCommand AddCommand { get; protected set; }
        public DelegateCommand EditCommand { get; protected set; }
        public DelegateCommand DeleteCommand { get; protected set; }
        public DelegateCommand SaveCommand { get; protected set; }
        public DelegateCommand CancelCommand { get; protected set; }
        public DelegateCommand RefreshCommand { get; protected set; }

        protected CrudViewModelBase()
        {
            Items = new ObservableCollection<T>();
            InitializeCommands();
        }

        protected virtual void InitializeCommands()
        {
            AddCommand = new DelegateCommand(OnAdd, CanAdd);
            EditCommand = new DelegateCommand(OnEdit, CanEdit);
            DeleteCommand = new DelegateCommand(OnDelete, CanDelete);
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CancelCommand = new DelegateCommand(OnCancel);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        protected abstract void OnAdd();
        protected abstract void OnEdit();
        protected abstract void OnDelete();
        protected abstract void OnSave();
        protected abstract void OnCancel();
        protected abstract void OnRefresh();

        protected virtual bool CanAdd() => !IsEditing;
        protected virtual bool CanEdit() => SelectedItem != null && !IsEditing;
        protected virtual bool CanDelete() => SelectedItem != null && !IsEditing;
        protected virtual bool CanSave() => IsEditing;
    }
}