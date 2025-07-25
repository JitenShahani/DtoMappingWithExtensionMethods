﻿namespace ManualDtoMappingDemo.Entities;

public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}