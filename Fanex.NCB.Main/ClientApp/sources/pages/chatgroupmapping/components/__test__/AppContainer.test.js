import React from 'react';
import { AppContainer } from '../AppContainer';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("AppContainer", () => { 
    describe("render", () => { 
        it("hide updatePopup", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    isShowed: false
                }
            };

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);

            // Assert
            expect(appContainer).toMatchSnapshot();
        });

        it("show updatePopup", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    isShowed: true
                }
            };

            // Act
            let appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);

            // Assert
            expect(appContainer).toMatchSnapshot();
        });
    });

    describe("closeUpdatePopupHandler", () => { 
        it("when UpdatePopup is showed", () => { 
            // Arrange
            let props = {
                closeEditPopup: jest.fn(),
                refreshUpdateData: jest.fn(),
                updatePopupData: {
                    isShowed: true
                },
                getGridData: jest.fn(),
                gridParams: "some data"
            };
            let appContainer = new AppContainer(props);

            // Act
            appContainer.closeUpdatePopupHandler();

            // Assert
            expect(props.closeEditPopup).toHaveBeenCalled();
            expect(props.refreshUpdateData).toHaveBeenCalled();
            expect(props.getGridData).toBeCalledTimes(0);
        });

        it("when UpdatePopup is not showed", () => { 
            // Arrange
            let props = {
                closeEditPopup: jest.fn(),
                refreshUpdateData: jest.fn(),
                updatePopupData: {
                    isShowed: false
                },
                getGridData: jest.fn(),
                gridParams: "some data"
            };
            let appContainer = new AppContainer(props);

            // Act
            appContainer.closeUpdatePopupHandler();

            // Assert
            expect(props.closeEditPopup).toHaveBeenCalled();
            expect(props.refreshUpdateData).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalledWith(props.gridParams);
        });
    });
});