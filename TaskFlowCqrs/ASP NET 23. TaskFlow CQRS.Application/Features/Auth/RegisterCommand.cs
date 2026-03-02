using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public record RegisterCommand(RegisterRequestDto RequestDto) : IRequest<AuthResponseDto>;