using System;
using BookHaeven.Dtos.User;
using BookHaeven.Models;

namespace BookHaeven.Mappers;

public static class UserMapper
{
    public static User ToUserFromRegisterDto(this RegisterDto registerDto)
    {
        return new User {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
        };

    }

}
