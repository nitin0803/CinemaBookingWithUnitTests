﻿using System.Text;
using Domain.Accessor;
using Domain.Models;
using Domain.Enums;

namespace Service.Screen;

public class ScreenService(ICinemaAccessor cinemaAccessor) : IScreenService
{
    public void Show(string currentBookingId)
    {
        var cinema = cinemaAccessor.GetCinema();
        var seatsPerRow = cinema.SeatsPerRow;
        
        Console.WriteLine($"Booking id: {currentBookingId}");
        Console.WriteLine("Selected seats: ");
        Console.WriteLine();
        
        Console.WriteLine("         S C R E E N                  ");
        var separator = new StringBuilder();
        for (var i =0; i <= seatsPerRow*3 + 2; i++)
        {
            separator.Append('-');
        }
        Console.WriteLine(separator);
        var hallLayout = cinema.HallLayout;
        var rowLayOuts = hallLayout.RowLayOuts;
        foreach (var rowLayOut in rowLayOuts)
        {
            Console.Write($"{rowLayOut.RowLabel} ");
            foreach (var seat in rowLayOut.Seats)
            {
                var seatSymbol = GetSeatSymbol(seat, currentBookingId);
                Console.Write(seatSymbol);
            }
            Console.WriteLine();
        }
        var seatNumber = new StringBuilder();
        var anyRowLayout = rowLayOuts.First();
        foreach (var seat in anyRowLayout.Seats)
        {
            seatNumber.Append(seat.SeatNumber + "  ");
        }
        Console.Write("   " + seatNumber);
        Console.WriteLine();
    }

    private static string GetSeatSymbol(Seat seat, string currentBookingId)
    {
        return seat switch
        {
            { Status: SeatStatus.Empty } => " . ",
            { BookingId: var bookingId } when (bookingId == currentBookingId) => " o ",
            _ => " # "
        };
    }
}