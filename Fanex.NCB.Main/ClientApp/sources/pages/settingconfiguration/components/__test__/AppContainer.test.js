import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import DataGrid from '../DataGrid';
import Filter from '../Filter';
import EditSettingType from '../EditSettingType';
import NewSettingType from '../NewSettingType';
import { toast } from 'react-toastify';
import Labels from '../../../common/Labels';
import ConfirmPopup from '../../../common/confirm-popup/ConfirmPopup';

describe('AppContainer', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    });

    beforeEach(() => {
        const haveHostPermission = document.createElement('div');
        haveHostPermission.setAttribute('id', 'have-host-permission');
        haveHostPermission.setAttribute('checked', true);
        document.body.appendChild(haveHostPermission);
    });

    afterEach(() => {
        const haveHostPermission = document.getElementById('have-host-permission');
        if (haveHostPermission) {
            document.body.removeChild(haveHostPermission);
        }
    });

    const initProps = {
        createSettingTypePopup: { show: true },
        editSettingTypePopup: { show: true },
        deleteSettingTypePopup: { show: true },
        gridParams: { pageIndex: 1, pageSize: 100 },
        getSettingTypes: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().componentDidMount();

            // Assert
            expect(props.getSettingTypes).toHaveBeenCalled();
        });
    });

    describe('handleOpenCreateSettingPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateSettingPopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleOpenCreateSettingPopup();

            // Assert
            expect(props.setCreateSettingPopup).toHaveBeenCalledWith({ show: true });
        });
    });

    describe('handleCloseCreateSettingTypePopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateSettingPopup: jest.fn(),
                setCreateSettingTypeResult: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateSettingTypePopup();

            // Assert
            expect(props.setCreateSettingPopup).toHaveBeenCalledWith({ show: false });
            expect(props.setCreateSettingTypeResult).toHaveBeenCalledWith(null);
        });
    });

    describe('handleCreateSuccess', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                getGridData: jest.fn()
            };
            const handleCloseCreateSettingTypePopup = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateSettingTypePopup = handleCloseCreateSettingTypePopup;
            container.instance().handleCreateSuccess();

            // Assert
            expect(handleCloseCreateSettingTypePopup).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 100 });
        });
    });

    describe('handleEditSuccess', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditSettingTypePopup: jest.fn(),
                setEditSettingTypeResult: jest.fn(),
                getGridData: jest.fn()
            };
            const handleCloseCreateSettingTypePopup = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateSettingTypePopup = handleCloseCreateSettingTypePopup;
            container.instance().handleEditSuccess();

            // Assert
            expect(props.setEditSettingTypePopup).toHaveBeenCalledWith({ show: false });
            expect(props.setEditSettingTypeResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 100 });
        });
    });

    describe('handleDeleteSettingType', () => {
        it('should call exactly props function when successful', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: true });
            const props = {
                ...initProps,
                deleteSettingTypePopup: { type: 'chat' },
                setGridParams: jest.fn(),
                closeDeleteSettingTypePopup: jest.fn(),
                getGridData: jest.fn(),
                deleteSettingType: jest.fn(() => promise)
            };

            toast.success = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteSettingType();

            // Assert
            expect(props.getGridData).toHaveBeenCalled();
            expect(toast.success).toHaveBeenCalledWith(Labels.DeleteMsgContentsTypeSuccessfully);
            expect(props.closeDeleteSettingTypePopup).toHaveBeenCalled();
        });

        it('should call exactly props function when error', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: false, errors: [{ errorMessage: 'err' }] });
            const props = {
                ...initProps,
                deleteSettingTypePopup: { type: 'chat' },
                setGridParams: jest.fn(),
                closeDeleteSettingTypePopup: jest.fn(),
                getGridData: jest.fn(),
                deleteSettingType: jest.fn(() => promise)
            };

            toast.error = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteSettingType();

            // Assert
            expect(toast.error).toHaveBeenCalledWith('err');
        });

        it('should call exactly props function when unSuccessful', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: false, errors: [], message: 'false' });
            const props = {
                ...initProps,
                deleteSettingTypePopup: { type: 'chat' },
                setGridParams: jest.fn(),
                closeDeleteSettingTypePopup: jest.fn(),
                getGridData: jest.fn(),
                deleteSettingType: jest.fn(() => promise)
            };

            toast.error = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteSettingType();

            // Assert
            expect(toast.error).toHaveBeenCalledWith('false');
        });
    });

    describe('render', () => {
        it('should render correctly', () => {
            // Arrange
            const props = { ...initProps };
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(Filter)).toHaveLength(1);
            expect(container.find(DataGrid)).toHaveLength(1);
            expect(container.find(NewSettingType)).toHaveLength(1);
            expect(container.find(EditSettingType)).toHaveLength(1);
            expect(container.find(ConfirmPopup)).toHaveLength(1);
        });
    });
});