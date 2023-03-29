﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client;

public partial class MainWindow : Window
{
    private bool xTurn = true;
    private int[,] board = new int[3, 3];

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        int row = Grid.GetRow(button);
        int column = Grid.GetColumn(button);

        if (xTurn)
        {
            button.Content = "X";
            board[row, column] = 1;
        }
        else
        {
            button.Content = "O";
            board[row, column] = 2;
        }

        button.IsEnabled = false;
        xTurn = !xTurn;

        CheckForWinner();
    }

    private void CheckForWinner()
    {

        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
            {
                ShowWinner(board[i, 0]);
                return;
            }
        }

        
        for (int i = 0; i < 3; i++)
        {
            if (board[0, i] != 0 && board[0, i] == board[1, i] && board[1, i] == board[2, i])
            {
                ShowWinner(board[0, i]);
                return;
            }
        }

        
        if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
        {
            ShowWinner(board[0, 0]);
            return;
        }

        if (board[0, 2] != 0 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
        {
            ShowWinner(board[0, 2]);
            return;
        }

        bool isTie = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0)
                {
                    isTie = false;
                    break;
                }
            }

            if (!isTie)
            {
                break;
            }
        }

        if (isTie)
        {
            ShowTie();
        }
    }

    private void ShowWinner(int player)
    {
        MessageBox.Show($"Player {(player == 1 ? "X" : "O")} wins!");
        ResetGame();
    }

    private void ShowTie()
    {
        MessageBox.Show("It's a tie!");
        ResetGame();
    }

    private void ResetGame()
    {
        board = new int[3, 3];
        xTurn = true;

        foreach (var element in grid.Children)
        {
            if (element is Button button)
            {
                button.Content = "";
                button.IsEnabled = true;
            }
        }
    }
}