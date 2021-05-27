import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import Filter from '../Filter';
import DataGrid from '../DataGrid';
import EditPopup from '../EditPopup';

describe('AppContainer', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('render', () => {
        it('should render correctly default components', () => {
            // Act
            const container = shallow(<AppContainer />);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum).length).toBe(1);
            expect(container.find(Filter).length).toBe(1);
            expect(container.find(DataGrid).length).toBe(1);
            expect(container.find(EditPopup).length).toBe(0);
        });

        it('should render correctly EditPopup component', () => {
            // Arrange
            let props = {
                editPopupData: { isShowed: true }
            };

            // Act
            const container = shallow(<AppContainer {...props} />);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum).length).toBe(1);
            expect(container.find(Filter).length).toBe(1);
            expect(container.find(DataGrid).length).toBe(1);
            expect(container.find(EditPopup).length).toBe(1);
        });
    });

    describe('handleCloseEditPopup', () => {
        it('popup is showed - should call exactly functions when close popup', () => {
            // Arrange
            let props = {
                editPopupData: {
                    isShowed: true
                },
                gridParams: {
                    channelIds: '',
                    botIds: '',
                    pageIndex: 1,
                    pageSize: 100
                },
                setEditPopupData: jest.fn(),
                setEditResult: jest.fn(),
                getGridData: jest.fn()
            };
            let container = shallow(<AppContainer {...props} />).instance();

            // Act
            container.handleCloseEditPopup();

            // Assert
            expect(props.setEditPopupData).toHaveBeenCalledWith({ isShowed: false, editingItem: {} });
            expect(props.setEditResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toBeCalledTimes(0);
        });

        it('popup is not show - should call exactly functions when close popup', () => {
            // Arrange
            let props = {
                editPopupData: {
                    isShowed: false
                },
                gridParams: {
                    channelIds: '',
                    botIds: '',
                    pageIndex: 1,
                    pageSize: 100
                },
                setEditPopupData: jest.fn(),
                setEditResult: jest.fn(),
                getGridData: jest.fn()
            };
            let container = shallow(<AppContainer {...props} />).instance();

            // Act
            container.handleCloseEditPopup();

            // Assert
            expect(props.setEditPopupData).toHaveBeenCalledWith({ isShowed: false, editingItem: {} });
            expect(props.setEditResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toHaveBeenCalledWith({
                channelIds: '',
                botIds: '',
                pageIndex: 1,
                pageSize: 100
            });
        });
    });
});