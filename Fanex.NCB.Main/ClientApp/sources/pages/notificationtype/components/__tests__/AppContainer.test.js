import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import DataGrid from '../DataGrid';
import CreateEditPopup from '../CreateEditPopup';
import DeletePopup from '../DeletePopup';

describe('AppContainer', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('handleOpenCreatePopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                setCreateEditPopupData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleOpenCreatePopup();

            // Assert
            expect(props.setCreateEditPopupData).toHaveBeenCalled();
        });
    });

    describe('handleCloseCreateEditPopup', () => {
        it('should call exactly props function when show is true', () => {
            // Arrange
            const props = {
                createEditPopupData: { isShowed: true },
                gridParams: { name: 'test' },
                setCreateEditPopupData: jest.fn(),
                setCreateEditResult: jest.fn(),
                setActiveRowIndex: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateEditPopup();

            // Assert
            expect(props.setCreateEditPopupData).toHaveBeenCalledWith({ isShowed: false });
            expect(props.setCreateEditResult).toHaveBeenCalledWith(null);
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(-1);
        });

        it('should call exactly props function when show is false', () => {
            // Arrange
            const props = {
                createEditPopupData: { isShowed: false },
                gridParams: { name: 'test' },
                setCreateEditPopupData: jest.fn(),
                setCreateEditResult: jest.fn(),
                setActiveRowIndex: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateEditPopup();

            // Assert
            expect(props.setCreateEditPopupData).toHaveBeenCalledWith({ isShowed: false });
            expect(props.setCreateEditResult).toHaveBeenCalledWith(null);
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(-1);
            expect(props.getGridData).toHaveBeenCalledWith({ name: 'test' });
        });
    });

    describe('handleCloseDeletePopup', () => {
        it('should call exactly props function when show is true', () => {
            // Arrange
            const props = {
                deletePopupData: { isShowed: true },
                gridParams: { name: 'test' },
                setDeletePopupData: jest.fn(),
                setDeleteResult: jest.fn(),
                setActiveRowIndex: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseDeletePopup();

            // Assert
            expect(props.setDeletePopupData).toHaveBeenCalledWith({ isShowed: false });
            expect(props.setDeleteResult).toHaveBeenCalledWith(null);
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(-1);
        });

        it('should call exactly props function when show is false', () => {
            // Arrange
            const props = {
                deletePopupData: { isShowed: false },
                gridParams: { name: 'test' },
                setDeletePopupData: jest.fn(),
                setDeleteResult: jest.fn(),
                setActiveRowIndex: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseDeletePopup();

            // Assert
            expect(props.setDeletePopupData).toHaveBeenCalledWith({ isShowed: false });
            expect(props.setDeleteResult).toHaveBeenCalledWith(null);
            expect(props.setActiveRowIndex).toHaveBeenCalledWith(-1);
            expect(props.getGridData).toHaveBeenCalledWith({ name: 'test' });
        });
    });

    describe('render', () => {
        it('should render correctly default components', () => {
            // Arrange
            const props = {
                isHost: true,
                createEditPopupData: { isShowed: true },
                deletePopupData: { isShowed: true }
            };
            
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(DataGrid)).toHaveLength(1);
            expect(container.find(CreateEditPopup)).toHaveLength(1);
            expect(container.find(DeletePopup)).toHaveLength(1);
        });
    });
});