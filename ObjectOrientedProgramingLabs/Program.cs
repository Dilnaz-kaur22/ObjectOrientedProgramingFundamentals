using System;
using System.Collections.Generic;

class Hotel
{
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Room> Rooms { get; set; }
    public List<Client> Clients { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Hotel(string name, string address)
    {
        Name = name;
        Address = address;
        Rooms = new List<Room>();
        Clients = new List<Client>();
        Reservations = new List<Reservation>();
    }

    public void AddRoom(Room room)
    {
        Rooms.Add(room);
    }

    public void AddClient(Client client)
    {
        Clients.Add(client);
    }

    public void MakeReservation(Client client, Room room, DateTime date, int occupants)
    {
        if (!room.Occupied)
        {
            // Create a new reservation
            Reservation reservation = new Reservation(date, occupants, client, room);
            reservation.IsCurrent = true;

            // Update the room and client with the reservation
            room.Occupied = true;
            room.Reservations.Add(reservation);
            client.Reservations.Add(reservation);

            // Add the reservation to the hotel's reservation list
            Reservations.Add(reservation);

            Console.WriteLine($"Reservation successfully made for {client.Name} in room {room.Number}.");
        }
        else
        {
            Console.WriteLine($"Room {room.Number} is already occupied.");
        }
    }

    public void CheckRoomAvailability(Room room)
    {
        Console.WriteLine($"Room {room.Number} Availability: {(room.Occupied ? "Occupied" : "Vacant")}");
    }

    public void PrintAllReservations()
    {
        Console.WriteLine("All Reservations:");
        foreach (var reservation in Reservations)
        {
            Console.WriteLine($"Client: {reservation.Client.Name} | Room: {reservation.Room.Number} | Date: {reservation.Date} | Occupants: {reservation.Occupants}");
        }
    }
}

class Room
{
    public string Number { get; set; }
    public int Capacity { get; set; }
    public bool Occupied { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Room(string number, int capacity)
    {
        Number = number;
        Capacity = capacity;
        Occupied = false;
        Reservations = new List<Reservation>();
    }
}

class Client
{
    public string Name { get; set; }
    public long CreditCard { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Client(string name, long creditCard)
    {
        Name = name;
        CreditCard = creditCard;
        Reservations = new List<Reservation>();
    }
}

class Reservation
{
    public DateTime Date { get; set; }
    public int Occupants { get; set; }
    public bool IsCurrent { get; set; }
    public Client Client { get; set; }
    public Room Room { get; set; }

    public Reservation(DateTime date, int occupants, Client client, Room room)
    {
        Date = date;
        Occupants = occupants;
        IsCurrent = false;
        Client = client;
        Room = room;
    }
}

class Program
{
    static void Main()
    {
        // Create a hotel
        Hotel hotel = new Hotel("Grand Hotel", "124 Main St");

        // Create rooms
        Room room1 = new Room("101", 2);
        Room room2 = new Room("102", 1);
        Room room3 = new Room("103", 3);
        Room room4 = new Room("104", 2);
        Room room5 = new Room("105", 3);
        Room room6 = new Room("106", 4);
        Room room7 = new Room("107", 4);
        Room room8 = new Room("108", 2);


        // Add rooms to the hotel
        hotel.AddRoom(room1);
        hotel.AddRoom(room2);
        hotel.AddRoom(room3);
        hotel.AddRoom(room4);
        hotel.AddRoom(room5);
        hotel.AddRoom(room6);
        hotel.AddRoom(room7);
        hotel.AddRoom(room8);


        // Create clients
        Client client1 = new Client("Janey Doe", 123456789);
        Client client2 = new Client("Jane Smith", 987654321);
        Client client3 = new Client("Sam Sith", 987654321);
        Client client4 = new Client("Jainny Smith", 987654321);
        Client client5 = new Client("Jana John", 987654321);
        Client client6 = new Client("Simpson Son", 987654321);


        // Add clients to the hotel
        hotel.AddClient(client1);
        hotel.AddClient(client2);
        hotel.AddClient(client3);
        hotel.AddClient(client4);
        hotel.AddClient(client5);
        hotel.AddClient(client6);

        // Make reservations
        hotel.MakeReservation(client1, room1, DateTime.Now, 2);
        hotel.MakeReservation(client2, room2, DateTime.Now.AddDays(2), 1);
        hotel.MakeReservation(client3, room3, DateTime.Now.AddDays(5), 3);
        hotel.MakeReservation(client4, room4, DateTime.Now.AddDays(7), 2);

        // Check room availability
        hotel.CheckRoomAvailability(room1);
        hotel.CheckRoomAvailability(room2);
        hotel.CheckRoomAvailability(room3);
        hotel.CheckRoomAvailability(room4);
        hotel.CheckRoomAvailability(room5);
        hotel.CheckRoomAvailability(room6);


        // Print all reservations
        hotel.PrintAllReservations();
    }
}
