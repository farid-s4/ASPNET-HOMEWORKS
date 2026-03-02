using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Features.Auth;

public record RevokeTokenCommand(RefreshTokenRequest RefreshTokenRequest) : IRequest;
