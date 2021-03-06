﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wazera.Data;
using Wazera.Model;

namespace Wazera.Kanban
{
    public class KanbanColumn : ListBox
    {
        public StatusData Data { get; set; }

        private KanbanBoard kanbanBoard;
        private Border border;
        private Label header;

        public KanbanColumn(KanbanBoard kanbanBoard, StatusData data)
        {
            Data = data;
            TaskModel.FillStatus(Data);
            this.kanbanBoard = kanbanBoard;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Margin = new Thickness(5);
            Background = Brushes.LightGray;
            BorderThickness = new Thickness(0);
            Focusable = false;

            border = new Border
            {
                Margin = new Thickness(5),
                Background = Brushes.LightGray,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(10),
                Child = this
            };
            if(!Data.IsBacklog)
            {
                PreviewDragEnter += (sender, e) => kanbanBoard.ItemPreviewShow(this);
                PreviewDragLeave += (sender, e) => { if (!IsCursorInside(e)) kanbanBoard.ItemPreviewRemove(); };
                Drop += (sender, e) => kanbanBoard.ItemDrop(sender, e);
                AllowDrop = true;
            }

            AddHeader();
        }

        public bool IsCursorInside(DragEventArgs e)
        {
            Point point = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, point);
            return result != null && !(result.VisualHit is ScrollViewer);
        }

        public Border AsBorderedColumn()
        {
            return border;
        }

        public int GetCardCount()
        {
            return Items.Count - 1;
        }

        private void AddHeader()
        {
            header = new Label
            {
                Margin = new Thickness(3),
                Padding = new Thickness(5),
                MinWidth = 180,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkSlateGray,
                Background = Brushes.LightGray,
                Focusable = false
            };
            ListBoxItem item = new ListBoxItem
            {
                Content = header,
                IsEnabled = false
            };
            item.Selected += (sender, e) => item.IsSelected = false;
            Items.Add(item);
            UpdateHeader();
        }

        public void UpdateHeader()
        {
            int cardCount = GetCardCount();
            header.Content = Data.Title.ToUpper() + " (" + cardCount + ")";
            if(Data.HasCardMinimum() && cardCount < Data.MinCards)
            {
                border.BorderBrush = Brushes.LightSkyBlue;
            }
            else if(Data.HasCardMaximum() && cardCount > Data.MaxCards)
            {
                border.BorderBrush = Brushes.LightSalmon;
            }
            else
            {
                border.BorderBrush = Brushes.White;
            }

            if(Data.IsBacklog)
            {
                foreach(object item in Items)
                {
                    if(item is KanbanTaskCard)
                    {
                        KanbanTaskCard taskCard = item as KanbanTaskCard;
                        bool alternating = Items.IndexOf(item) % 2 == 1;
                        taskCard.SetDefaultBrush(alternating ? Brushes.White : new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)));
                    }
                }
            }
        }

        public void SaveData()
        {
            Data.Tasks.Clear();
            for(int index = 1; index <= GetCardCount(); index++)
            {
                KanbanTaskCard item = (KanbanTaskCard) Items.GetItemAt(index);
                item.Data.Status = Data;
                Data.Tasks.Add(item.Data);

                new TaskModel(item.Data)
                {
                    SortOrder = Data.Tasks.IndexOf(item.Data)
                }.Save();
            }
        }

        public void AddRow(TaskData task)
        {
            AddRow(task, Items.Count);
        }

        public void AddRow(TaskData task, int index)
        {
            task.Status = Data;
            KanbanTaskCard item = new KanbanTaskCard(kanbanBoard, task);
            Items.Insert(index, item);
            UpdateHeader();
        }
    }
}
