﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Messagemanagementservice;
using MessageManagementService.Interface;
using MessageManagementService.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MessageManagementService.ProtoContracts
{
    public class MessageServicePc:MessageService.MessageServiceBase
    {

        private readonly IMessageManagement _messageManagement;
        private readonly IDBCacheService _dbcacheService;
        public MessageServicePc(IMessageManagement messageManagement, IDBCacheService cacheService)
        {
            _messageManagement = messageManagement;
            _dbcacheService = cacheService;
        }
        public override Task<ApiResponse> StoreUserMessage(userMessage request, ServerCallContext context)
        {
            var userMessageRequest = new UserMessage
            {
                cachedMessageId = request.CachedMessageId,
                messageContent = request.MessageContent,
                fileType = request.FileType,
                toUserId = request.ToUserId,
                fromUserId = request.FromUserId,
                hasFile = request.HasFile,
                fileUrl = request.FileUrl,
            };

            var response = _messageManagement.StoreUserMessage(userMessageRequest);
            return Task.FromResult(new ApiResponse
            {
                Success = response.Result.Success,
                Data = response.Result.Data,
                ErrorMessage = response.Result.ErrorMessage == null ? string.Empty : response.Result.ErrorMessage,
            });
        }
        public override Task<ApiResponse> StoreCachedMessage(storeCachedMessageRequest request, ServerCallContext context)
        {
            var userRequest = request.CachedMessageIds.ToList();

            var response = _messageManagement.StoreCachedMessage(userRequest);
            return Task.FromResult(new ApiResponse
            {
                ErrorMessage = response.ErrorMessage==null ? string.Empty : response.ErrorMessage,
                Data = response.Data,
                Success = response.Success
            });
        }
        
    }
}
