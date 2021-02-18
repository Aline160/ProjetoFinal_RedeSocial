﻿using InstaGama.Domain.Entities;
using InstaGama.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InstaGama.Repositories
{
    public class PostageRepository : IPostageRepository
    {
        private readonly IConfiguration _configuration;

        public PostageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Postage>> GetPostageByUserIdAsync(int userId)
        {
            using (var con = new SqlConnection(_configuration["ConnectionString"]))
            {
                var sqlCmd = @$"SELECT Id,
	                                   UsuarioId,
                                       Texto,
                                       Criacao
                                FROM 
	                                Postagem
                                WHERE 
	                                UsuarioId= '{userId}'";

                using (var cmd = new SqlCommand(sqlCmd, con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    var reader = await cmd
                                        .ExecuteReaderAsync()
                                        .ConfigureAwait(false);

                    var postagesForUser = new List<Postage>();

                    while (reader.Read())
                    {
                        var postage = new Postage(int.Parse(reader["Id"].ToString()),
                                                    reader["Texto"].ToString(),
                                                    int.Parse(reader["UsuarioId"].ToString()),
                                                    DateTime.Parse(reader["Criacao"].ToString()));

                        postagesForUser.Add(postage);
                    }

                    return postagesForUser;
                }
            }
        }

        public async Task<int> InsertAsync(Postage postage)
        {
            using (var con = new SqlConnection(_configuration["ConnectionString"]))
            {
                var sqlCmd = @"INSERT INTO
                                Postagem (UsuarioId,
                                           Texto,
                                           Criacao)
                                VALUES (@usuarioId,
                                        @texto,
                                        @criacao); SELECT scope_identity();";

                using (var cmd = new SqlCommand(sqlCmd, con))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("usuarioId", postage.UserId);
                    cmd.Parameters.AddWithValue("texto", postage.Text);
                    cmd.Parameters.AddWithValue("criacao", postage.Created);

                    con.Open();
                    var id = await cmd
                                    .ExecuteScalarAsync()
                                    .ConfigureAwait(false);

                    return int.Parse(id.ToString());
                }
            }
        }

        public async Task<List<string>> GetGalleryByUserIdAsync(int userId)
        {
            using (var con = new SqlConnection(_configuration["ConnectionString"]))
            {
                var sqlCmd = @$"SELECT Foto
                                FROM 
	                                Postagem
                                WHERE 
	                                UsuarioId= '{userId}'
                                    AND Foto <> ''";

                using (var cmd = new SqlCommand(sqlCmd, con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    var reader = await cmd
                                        .ExecuteReaderAsync()
                                        .ConfigureAwait(false);

                    var GalleryForUser = new List<string>();

                    while (reader.Read())
                    {
                        var image = reader["Foto"].ToString();

                        GalleryForUser.Add(image);
                    }

                    return GalleryForUser;
                }
            }
        }
    }
}
