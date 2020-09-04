﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Inmobiliaria.Models
{
    public class RepositorioInquilino :RepositorioBase
    {
		public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inquilino e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Direccion, Email, LugarTrabajo, NombreGarante,ApellidoGarante DniGarante, TelefonoGarante, DireccionGarante) " +
					$"VALUES (@nombre, @apellido, @dni, @telefono,@direccion, @email, @lugarTrabajo, @nombreGarante,@apellidoGarante, @dniGarante, @telefonoGarante, @direccionGarante)";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@dni", e.Dni);
					command.Parameters.AddWithValue("@telefono", e.Telefono);
					command.Parameters.AddWithValue("@direccion", e.Direccion);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@lugartrabajo", e.LugarTrabajo);
					command.Parameters.AddWithValue("@nombreGarante", e.NombreGarante);
		            command.Parameters.AddWithValue("@apellidoGarante", e.ApellidoGarante);
					command.Parameters.AddWithValue("@dniGarante", e.DniGarante);
					command.Parameters.AddWithValue("@telefonoGarante", e.TelefonoGarante);
					command.Parameters.AddWithValue("@direccionGarante", e.DireccionGarante);
					connection.Open();
					res = command.ExecuteNonQuery();
					command.CommandText = "SELECT SCOPE_IDENTITY()";
					e.IdInquilino = (int)command.ExecuteScalar();
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inquilino WHERE IdInquilino = {id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Inquilino e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Inquilino SET " +
					$"Nombre=@nombre', Apellido=@apellido, Dni=@dni, Telefono=@telefono,Direccion=@direccion, Email=@email,LugarTrabajo=@lugar_trabajo, NombreGarante=@nombre_garante,ApellidoGarante=@apellidoGarante, DniGarante=@dni_garante, TelefonoGarante=@telefono_garante, DireccionGarante=@direccion_garante" +
					$"WHERE IdInquilino = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@dni", e.Dni);
					command.Parameters.AddWithValue("@telefono", e.Telefono);
					command.Parameters.AddWithValue("@direccion", e.Direccion);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@lugar_trabajo", e.LugarTrabajo);
					command.Parameters.AddWithValue("@nombre_garante", e.NombreGarante);
				    command.Parameters.AddWithValue("@apellido_garante", e.ApellidoGarante);
					command.Parameters.AddWithValue("@dni_garante", e.DniGarante);
					command.Parameters.AddWithValue("@telefono_garante", e.TelefonoGarante);
					command.Parameters.AddWithValue("@direccion_garante", e.DireccionGarante);
					command.Parameters.AddWithValue("@id", e.IdInquilino);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono,Direccion, Email, LugarTrabajo, NombreGarante, ApellidoGarante, DniGarante, TelefonoGarante, DireccionGarante" +
					$" FROM Inquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							IdInquilino = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Direccion = reader.GetString(5),
							Email = reader.GetString(6),
							LugarTrabajo= reader.GetString(7),
							NombreGarante = reader.GetString(8),
				            ApellidoGarante = reader.GetString(9),
							DniGarante = reader.GetString(10),
							TelefonoGarante = reader.GetString(11),
							DireccionGarante = reader.GetString(12),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inquilino ObtenerPorId(int id)
		{
			Inquilino p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono,Direccion, Email, LugarTrabajo, NombreGarante, ApellidoGarante, DniGarante, TelefonoGarante, DireccionGarante FROM Inquilino" +
					$" WHERE IdInquilino=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Direccion = reader.GetString(5),
							Email = reader.GetString(6),
							LugarTrabajo = reader.GetString(7),
							NombreGarante = reader.GetString(8),
		                    ApellidoGarante = reader.GetString(9),
							DniGarante = reader.GetString(10),
							TelefonoGarante = reader.GetString(11),
							DireccionGarante = reader.GetString(12),
						};
						return p;
					}
					connection.Close();
				}
			}
			return p;
		}

	}
}