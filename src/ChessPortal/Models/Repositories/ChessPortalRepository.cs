using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessPortal.DataInterfaces;
using ChessPortal.Entities;
using ChessPortal.Models.Chess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ChessPortal.Models.Repositories
{
    public class ChessPortalRepository : IChessPortalRepository
    {
        private readonly ChessPortalContext _context;

        public ChessPortalRepository(ChessPortalContext context)
        {
            _context = context;
        }

        public void AddChallenge(ChallengeEntity challenge)
        {
            _context.Challenges.Add(challenge);
        }

        public IEnumerable<ChallengeEntity> GetChallenges(string playerId)
        {
            return
                _context.Challenges.Where(
                    c => c.PlayerId != playerId && _context.AcceptedChallenges.All(ac => ac.ChallengeId != c.Id));
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public bool ChallengeExists(Guid challengeId)
        {
            return _context.Challenges.Any(c => c.Id == challengeId);
        }

        public bool ChallengeIsAccepted(Guid challengeId)
        {
            return _context.AcceptedChallenges.Any(ac => ac.ChallengeId == challengeId);
        }

        public bool ChallengeIsCreatedByPlayer(Guid challengeId, string playerId)
        {
            return _context.Challenges.Any(c => c.Id == challengeId && c.PlayerId == playerId);
        }

        public void AcceptChallenge(Guid challengeId, string playerId)
        {
            _context.AcceptedChallenges.Add(new ChallengeAcceptEntity
            {
                ChallengeId = challengeId,
                PlayerId = playerId
            });
        }

        public IEnumerable<ChallengeEntity> GetAcceptedChallengesForPlayer(string playerId)
        {
            return _context.Challenges.Include(c => c.Moves).Include(c => c.DrawRequests)
                .Where(
                    c =>
                        c.PlayerId == playerId ||
                        _context.AcceptedChallenges.FirstOrDefault(ac => ac.PlayerId == playerId) != null).ToList();
        }

        public bool ChallengeIsCreatedOrAcceptedByPlayer(Guid challengeId, string playerId)
        {
            return
                _context.AcceptedChallenges.Any(
                    ac =>
                        ac.ChallengeId == challengeId && (ac.PlayerId == playerId || ac.Challenge.PlayerId == playerId));
        }

        public ChallengeEntity GetChallenge(Guid challengeId)
        {
            var challenge = _context.Challenges.Where(c => c.Id == challengeId).Include(c => c.Moves).SingleOrDefault();
            challenge.Moves = challenge.Moves.OrderBy(m => m.MoveNumber).ToList();
            return challenge;
        }

        public ChessPlayer GetPlayerForChallenge(Guid challengeId, Color color)
        {
            return
                _context.AcceptedChallenges.Where(ac => ac.ChallengeId == challengeId)
                    .Select(ac => ac.Challenge.Color == color ? ac.Challenge.Player : ac.AcceptingPlayer)
                    .SingleOrDefault();
        }

        public void AddMove(MoveEntity move)
        {
            var challenge = GetChallenge(move.ChallengeId);
            move.MoveNumber = GetNextMoveNumber(challenge);
            move.MoveDate = DateTime.Now;
            _context.Add(move);
        }

        public void AddDrawRequest(DrawRequestEntity drawRequest)
        {
            _context.Add(drawRequest);
        }

        public bool DrawRequestIsMadeByPlayer(Guid challengeId, string playerId)
        {
            return _context.DrawRequests.Any(d => d.ChallengeId == challengeId && d.PlayerId == playerId);
        }

        public bool DrawRequestExists(Guid challengeId)
        {
            return _context.DrawRequests.Any(d => d.ChallengeId == challengeId);
        }

        public void DeleteDrawRequest(Guid challengeId)
        {
            _context.DrawRequests.Remove(_context.DrawRequests.FirstOrDefault(d => d.ChallengeId == challengeId));
        }

        public ChessProblemEntity GetChessProblemForPlayer(string playerId)
        {
            return _context.ChessProblems.SingleOrDefault(p => p.PlayerId == playerId);
        }

        public async Task<bool> UpdateUser(ChessPlayer user)
        {
            var store = new UserStore<ChessPlayer>(_context);
            return await store.UpdateAsync(user) == IdentityResult.Success;
        }

        int GetNextMoveNumber(ChallengeEntity challenge)
        {
            var moveNumber = challenge.Moves.LastOrDefault()?.MoveNumber;
            return moveNumber + 1 ?? 1;
        }
    }
}
