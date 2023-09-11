﻿using Microsoft.Extensions.DependencyInjection;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace XenobiaSoft.Sudoku.Helpers;

public static class ServiceCollectionExtensionMethods
{
	public static IServiceCollection RegisterGameServices(this IServiceCollection services)
	{
		services.AddTransient<ISudokuGame, SudokuGame>();
		services.AddTransient<IGameStateMemory, GameStateMemory>();
		services.AddSingleton<IPuzzleSolver, PuzzleSolver>();
		services.AddSingleton<IPuzzleGenerator, PuzzleGenerator>();

		typeof(SolverStrategy).Assembly
			.GetTypes()
			.Where(x => x.Name.EndsWith("Strategy") && !x.IsAbstract && !x.IsInterface)
			.ToList()
			.ForEach(x =>
			{
				foreach (var @interface in x.GetInterfaces())
				{
					services.AddTransient(@interface, x);
				}
			});

		return services;
	}
}