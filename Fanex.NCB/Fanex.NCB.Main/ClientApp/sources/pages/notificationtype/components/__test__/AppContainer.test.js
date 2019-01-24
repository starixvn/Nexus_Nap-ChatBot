import React from 'react';
import {AppContainer} from '../AppContainer';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('AppContainer', () => {   
    describe('render', () => {
        it('hide UpdatePopup, DeletePopup', () => {
            // Arrange
            let props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: false},
                deletePopupData: {isShowed: false}
            };        
            
            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            

            // Assert
            expect(appContainer).toMatchSnapshot();
        })

        it('show UpdatePopup', () => {
            // Arrange
            let props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: true},
                deletePopupData: {isShowed: false}
            };

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            
            
            // Assert
            expect(appContainer).toMatchSnapshot();
        })

        it('show DeletePopup', () => {
            // Arrange
            let props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: false},
                deletePopupData: {isShowed: true}
            };

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            
            
            // Assert
            expect(appContainer).toMatchSnapshot();
        })
      
    })

    describe("closeUpdatePopupHandler", () => {
        it("when UpdatePopup is showed", () => {
            // Arrange
            let props = {                
                closeNewEditPopup: jest.fn(),
                refreshUpdateData: jest.fn(),
                updateActiveRowIndex: jest.fn(),
                updatePopupData: {isShowed: true}
            };
            let appContainer = new AppContainer(props) ;

            // Act
            appContainer.closeUpdatePopupHandler();

            // Assert
            expect(props.closeNewEditPopup).toHaveBeenCalled();
            expect(props.refreshUpdateData).toHaveBeenCalled();
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(-1);            
        })

        it("when UpdatePopup is not showed", () => {
            // Arrange
            const gridParams = {pageIndex: 1, pageSize: 20};
            let props = {                
                closeNewEditPopup: jest.fn(),
                refreshUpdateData: jest.fn(),
                updateActiveRowIndex: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: false},
                gridParams: gridParams
            };
            let appContainer = new AppContainer(props);

            // Act
            appContainer.closeUpdatePopupHandler();

            // Assert
            expect(props.closeNewEditPopup).toHaveBeenCalled();
            expect(props.refreshUpdateData).toHaveBeenCalled();
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(-1);
            expect(props.getGridData).toHaveBeenCalledWith(gridParams);
        })
    })

    describe("closeDeletePopupHandler", () => {
        it("when deletePopup is showed", () => {
            // Arrange
            let props = {                
                closeDeletePopup: jest.fn(),
                updateActiveRowIndex: jest.fn(),
                deletePopupData: {isShowed: true}
            };
            let appContainer = new AppContainer(props) ;

            // Act
            appContainer.closeDeletePopupHandler();

            // Assert
            expect(props.closeDeletePopup).toHaveBeenCalled();
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(-1);
        })

        it("when deletePopup is not showed", () => {
            // Arrange
            const gridParams = {pageIndex: 1, pageSize: 20};
            let props = {                
                closeDeletePopup: jest.fn(),
                updateActiveRowIndex: jest.fn(),
                getGridData: jest.fn(),
                deletePopupData: {isShowed: false},
                gridParams: gridParams
            };
            let appContainer = new AppContainer(props) ;

            // Act
            appContainer.closeDeletePopupHandler();

            // Assert
            expect(props.closeDeletePopup).toHaveBeenCalled();
            expect(props.updateActiveRowIndex).toHaveBeenCalledWith(-1);
            expect(props.getGridData).toHaveBeenCalledWith(gridParams);
        })
    })
})