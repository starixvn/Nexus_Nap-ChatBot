import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import { toast } from 'react-toastify';
import Filter from '../Filter';
import DataGrid from '../DataGrid';
import Labels from '../../../common/Labels';
import NewMapping from '../NewMapping';
import EditMapping from '../EditMapping';
import ConfirmPopup from '../../../common/confirm-popup/ConfirmPopup';
import EditJobInfo from '../EditJobInfo';

describe('AppContainer', () => {

    afterAll(() => {
        jest.clearAllMocks();
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
        jobInfoFlag: { isEdit: true },
        createMappingPopup: { show: true },
        editMappingPopup: { show: true },
        deleteMappingPopup: { show: true },
        getSettingTypes: jest.fn(),
        getNotificationTypes: jest.fn(),
        getJobInfo: jest.fn(),
        gridParams: { id: 1, name: 'test' }
    };

    describe('componentDidMount', () => {
        it('should call props function exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().componentDidMount();

            // Assert
            expect(props.getSettingTypes).toHaveBeenCalled();
        });
    });

    describe('handleOpenCreateMappingPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateMappingPopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleOpenCreateMappingPopup();

            // Assert
            expect(props.setCreateMappingPopup).toHaveBeenCalledWith({ show: true });
        });
    });

    describe('handleCloseCreateMappingPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateMappingPopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateMappingPopup();

            // Assert
            expect(props.setCreateMappingPopup).toHaveBeenCalledWith({ show: false });
        });
    });

    describe('handleCloseEditMappingPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditMappingPopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseEditMappingPopup();

            // Assert
            expect(props.setEditMappingPopup).toHaveBeenCalledWith({ show: false });
        });
    });

    describe('handleCreateSuccess', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                getGridData: jest.fn()
            };
            const handleCloseCreateMappingPopup = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateMappingPopup = handleCloseCreateMappingPopup;
            container.instance().handleCreateSuccess();

            // Assert
            expect(handleCloseCreateMappingPopup).toHaveBeenCalled();
            expect(props.getGridData).toHaveBeenCalledWith({ id: 1, name: 'test' });
        });
    });

    describe('handleEditSuccess', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditMappingPopup: jest.fn(),
                setEditMappingResult: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleEditSuccess();

            // Assert
            expect(props.setEditMappingPopup).toHaveBeenCalledWith({ show: false });
            expect(props.setEditMappingResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toHaveBeenCalledWith({ id: 1, name: 'test' });    
        });
    });

    describe('handleDeleteMapping', () => {
        it('should call exactly props function when isSuccessful', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: true });
            const props = {
                ...initProps,
                jobInfo: { id: 12 },
                deleteMappingPopup: {
                    templateId: 1,
                    msgContentsType: 'test'
                },
                deleteMapping: jest.fn(() => promise),
                setGridParams: jest.fn(),
                closeDeleteMappingPopup: jest.fn(),
                getGridData: jest.fn()
            };
            toast.success = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteMapping();

            // Assert
            expect(props.getGridData).toHaveBeenCalled();
            expect(toast.success).toHaveBeenCalledWith(Labels.DeleteMappingSettingSuccessfully);
            expect(props.closeDeleteMappingPopup).toHaveBeenCalled();
        });
        
        it('should call exactly props function when not success and error', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: false, errors: [{ errorMessage: 'false' }] });
            const props = {
                ...initProps,
                jobInfo: { id: 12 },
                deleteMappingPopup: {
                    templateId: 1,
                    msgContentsType: 'test'
                },
                deleteMapping: jest.fn(() => promise),
                setGridParams: jest.fn(),
                closeDeleteMappingPopup: jest.fn(),
                getGridData: jest.fn()
            };
            toast.error = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteMapping();

            // Assert
            expect(toast.error).toHaveBeenCalledWith('false');
        });

        it('should call exactly props function when not success', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: false, message: 'false' });
            const props = {
                ...initProps,
                jobInfo: { id: 12 },
                deleteMappingPopup: {
                    templateId: 1,
                    msgContentsType: 'test'
                },
                deleteMapping: jest.fn(() => promise),
                setGridParams: jest.fn(),
                closeDeleteMappingPopup: jest.fn(),
                getGridData: jest.fn()
            };
            toast.error = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteMapping();

            // Assert
            expect(toast.error).toHaveBeenCalledWith('false');
        });
    });

    describe('render', () => {
        it('should render correctly default components', () => {
            // Arrange
            const props = { ...initProps };
            // Act
            const container = shallow(<AppContainer {...props}/>);

            // Assert
            expect(container).toMatchSnapshot();
            expect(container.find(NCBBreadcrum)).toHaveLength(1);
            expect(container.find(Filter)).toHaveLength(1);
            expect(container.find(DataGrid)).toHaveLength(1);
            expect(container.find(EditJobInfo)).toHaveLength(1);
            expect(container.find(NewMapping)).toHaveLength(1);
            expect(container.find(EditMapping)).toHaveLength(1);
            expect(container.find(ConfirmPopup)).toHaveLength(1);
        });
    });
});