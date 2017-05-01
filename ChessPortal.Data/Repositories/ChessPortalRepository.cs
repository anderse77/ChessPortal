using ChessPortal.Data.Entities;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Logic.Chess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessPortal.Data.Repositories
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

        public IEnumerable<ChallengeEntity> GetChallengesThatPlayerCanAccept(string playerId)
        {
            return
                _context.Challenges.Where(
                        c => c.PlayerId != playerId && _context.AcceptedChallenges.All(ac => ac.ChallengeId != c.Id))
                    .Include(c => c.Player);
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
                        (c.PlayerId == playerId  && 
                        _context.AcceptedChallenges.Any(ac => ac.ChallengeId == c.Id)) ||
                        _context.AcceptedChallenges.Any(ac => ac.ChallengeId == c.Id && ac.PlayerId == playerId)).ToList();
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
            var challenge = _context.Challenges.Where(c => c.Id == challengeId)
                .Include(c => c.Moves)
                .SingleOrDefault();
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
            return _context.ChessProblems.FirstOrDefault(p => p.PlayerId == playerId);
        }

        public void AddChessProblem(ChessProblemEntity chessProblem)
        {
            _context.ChessProblems.Add(chessProblem);
        }

        public void DeleteChessProblem (Guid id)
        {
            _context.ChessProblems.Remove(_context.ChessProblems.FirstOrDefault(d => d.Id == id));
        }

        public ChessPlayer GetPlayerById(string playerId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == playerId);
        }

        public bool ChessProblemExistsForPlayer(string playerId)
        {
            return _context.ChessProblems.Any(p => p.PlayerId == playerId);
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
