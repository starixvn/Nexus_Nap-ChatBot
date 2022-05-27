import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import { NCBBreadcrum } from '../../../common/breadcrumbs/breadcrumbs';
import { toast } from 'react-toastify';
import Labels from '../../../common/Labels';
import Filter from '../Filter';
import DataGrid from '../DataGrid';
import NewNotificationTemplate from '../NewNotificationTemplate';
import EditNotificationTemplate from '../EditNotificationTemplate';
import ConfirmPopup from '../../../common/confirm-popup/ConfirmPopup';
import MarkdownRules from '../../../common/markdown-rules';

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
        gridParams: [{ id: 1, name: 'test' }],
        createTemplatePopup: { show: true },
        editTemplatePopup: { show: true },
        deleteTemplatePopup: { show: true },
        markdownRulesPopup: { show: true },
        getNotificationTypes: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().componentDidMount();

            // Assert
            expect(props.getNotificationTypes).toHaveBeenCalled();
        });
    });

    describe('handleOpenCreateTemplatePopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateTemplatePopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleOpenCreateTemplatePopup();

            // Assert
            expect(props.setCreateTemplatePopup).toHaveBeenCalledWith({ show: true });
        })
    });

    describe('handleCloseCreateTemplatePopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateTemplatePopup: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateTemplatePopup();

            // Assert
            expect(props.setCreateTemplatePopup).toHaveBeenCalledWith({ show: false });
        });
    });

    describe('handleCreateSuccess', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {
                ...initProps,
                setCreateTemplateResult: jest.fn(),
                getGridData: jest.fn()
            };
            const handleCloseCreateTemplatePopup = jest.fn();

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleCloseCreateTemplatePopup = handleCloseCreateTemplatePopup;
            container.instance().handleCreateSuccess();

            // Assert
            expect(handleCloseCreateTemplatePopup).toHaveBeenCalled();
            expect(props.setCreateTemplateResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toHaveBeenCalledWith(props.gridParams);
        });
    });

    describe('handleEditSuccess', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditTemplatePopup: jest.fn(),
                setEditTemplateResult: jest.fn(),
                getGridData: jest.fn()
            };

            // Act
            const container = shallow(<AppContainer {...props}/>);
            container.instance().handleEditSuccess();

            // Assert
            expect(props.setEditTemplatePopup).toHaveBeenCalledWith({ show: false });
            expect(props.setEditTemplateResult).toHaveBeenCalledWith(null);
            expect(props.getGridData).toHaveBeenCalledWith(props.gridParams);
        });
    });

    describe('handleDeleteTemplate', () => {
        it('should call exactly props function when data is success', async () => {
            // Arrange
            const resolve = { message: 'test', isSuccessful: true };
            const promise = Promise.resolve(resolve);
            const props = {
                ...initProps,
                deleteTemplatePopup: { templateId: 1 },
                deleteTemplate: jest.fn(() => promise),
                closeDeleteTemplatePopup: jest.fn(),
                getGridData: jest.fn()
            };
            const toastSpy = jest.spyOn(toast, 'success');

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteTemplate();

            // Assert
            expect(props.getGridData).toHaveBeenCalled();
            expect(toastSpy).toHaveBeenCalledWith(Labels.DeleteTemplateSuccessfully);
            expect(props.closeDeleteTemplatePopup).toHaveBeenCalled();
        });

        it('should call exactly props function when data is not success', async () => {
            // Arrange
            const resolve = { message: 'False to delete data', isSuccessful: false };
            const promise = Promise.resolve(resolve);
            const props = {
                ...initProps,
                deleteTemplatePopup: { templateId: 1 },
                deleteTemplate: jest.fn(() => promise),
                closeDeleteTemplatePopup: jest.fn(),
                getGridData: jest.fn()
            };
            const toastSpy = jest.spyOn(toast, 'error');

            // Act
            const container = shallow(<AppContainer {...props}/>);
            await container.instance().handleDeleteTemplate();

            // Assert
            expect(toastSpy).toHaveBeenCalledWith('False to delete data');
            expect(props.closeDeleteTemplatePopup).toHaveBeenCalled();
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
            expect(container.find(NewNotificationTemplate)).toHaveLength(1);
            expect(container.find(EditNotificationTemplate)).toHaveLength(1);
            expect(container.find(ConfirmPopup)).toHaveLength(1);
            expect(container.find(MarkdownRules)).toHaveLength(1);
        });
    });
});