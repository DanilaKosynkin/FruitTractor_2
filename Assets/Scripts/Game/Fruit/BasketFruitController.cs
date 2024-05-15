﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Code
{
    public class BasketFruitController : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _parentButton;
        [SerializeField] private BasketContainer _basketContainer;

        private List<BasketButton> _basketButton = new List<BasketButton>();
        private FruitType _currentFruitType;
        private GridController _gridController;
        private GameManager _gameManager;
        private FruitController _fruitController;

        [Inject]
        private void Construct(GridController gridController, GameManager gameManager, FruitController fruitController)
        {
            _fruitController = fruitController;
            _gameManager = gameManager;
            _gridController = gridController;
            _fruitController.OnUpFruit += OnUpFruit;
        }

        private void OnDestroy()
        {
            _fruitController.OnUpFruit -= OnUpFruit;
        }

        public void Initialize()
        {
            CreateButtons(_gridController._fruits);
        }

        public void SetFruitType(FruitType typeFruit, BasketButton button)
        {
            foreach (BasketButton fruitButton in _basketButton)
            {
                fruitButton.ChangeInteractable(true);
            }
            
            button.ChangeInteractable(false);
            _currentFruitType = typeFruit;
        }

        private void OnUpFruit(FruitType upFruitType)
        {
            if (upFruitType != _currentFruitType)
            {
                _gameManager.LossGame();
            }
        }

        private void CreateButtons(List<FruitCell> fruits)
        {
            List<FruitType> allFruitTypes = fruits.Select(fruitCell => fruitCell._fruit._fruitType)
                .Distinct().ToList();
            foreach (FruitType fruitType in allFruitTypes)
            {
                BasketButton newButton = Instantiate(_basketContainer._basketButtonPrefab, _parentButton);
                BasketButtonData basketData = _basketContainer.GetBasketData(fruitType);
                newButton.SetupData(basketData._fruitType, basketData._fruitIcon, this);
                _basketButton.Add(newButton);
            }

            SetFruitType(allFruitTypes[0], _basketButton[0]);
        }
    }
}