import React from 'react';
import { EditJobInfo } from '../EditJobInfo';
import { shallow } from 'enzyme';
import { toast } from 'react-toastify';

describe('EditJobInfo', () => {

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
        jobInfo: {
            id: 1,
            status: 'check',
            previousMaxMessageId: 2,
            batchSize: 3
        }
    }

    describe('handleChangeSaveMode', () => {
        it('should show toast err when batchSize not a number', () => {
            // Arrange
            const props = { ...initProps };
            const toggleConfirmUpdateJobInfo = jest.fn();
            toast.error = jest.fn();

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().toggleConfirmUpdateJobInfo = toggleConfirmUpdateJobInfo;
            container.setState({
                batchSize: 'test',
                testingGroup: '1, 2'
            });
            container.instance().handleChangeSaveMode();

            // Assert
            expect(toggleConfirmUpdateJobInfo).toHaveBeenCalled();
            expect(toast.error).toHaveBeenCalledWith('Batch Size must be integer');
        });

        it('should show toast err when batchSize not a number', () => {
            // Arrange
            const props = {...initProps };
            const toggleConfirmUpdateJobInfo = jest.fn();
            toast.error = jest.fn();

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().toggleConfirmUpdateJobInfo = toggleConfirmUpdateJobInfo;
            container.setState({
                batchSize: '1',
                testingGroup: 'x,y'
            });
            container.instance().handleChangeSaveMode();

            // Assert
            expect(toggleConfirmUpdateJobInfo).toHaveBeenCalled();
            expect(toast.error).toHaveBeenCalledWith('Testing Group must be integer and separated by \',\' for multiple group.');
        });

        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setJobInfoData: jest.fn(),
                setJobInfoFlag: jest.fn()
            };
            const toggleConfirmUpdateJobInfo = jest.fn();

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().toggleConfirmUpdateJobInfo = toggleConfirmUpdateJobInfo;
            container.setState({
                batchSize: '1',
                testingGroup: '1'
            });
            container.instance().handleChangeSaveMode();

            // Assert
            expect(props.setJobInfoData).toHaveBeenCalled();
        });
    });

    describe('handleChange', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: {
                    name: 'site',
                    value: 123
                }
            };

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().handleChange(mockEvent);

            // Assert
            expect(container.state().site).toEqual(123);
        });
    });

    describe('handleConfirmUpdateJobInfoPopup', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().handleConfirmUpdateJobInfoPopup();

            // Assert
            expect(container.state().confirmUpdateJobInfo).toEqual(true);
        });
    });

    describe('toggleConfirmUpdateJobInfo', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {
                ...initProps,
                setJobInfoFlag: jest.fn()
            };

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().toggleConfirmUpdateJobInfo();

            // Assert
            expect(container.state().confirmUpdateJobInfo).toEqual(false);
            expect(props.setJobInfoFlag).toHaveBeenCalledWith({ isEdit: false });
        });
    });

    describe('handleCancel', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setJobInfoFlag: jest.fn()
            };

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.instance().handleCancel();

            // Assert
            expect(props.setJobInfoFlag).toHaveBeenCalledWith({ isEdit: false });
        });
    });

    describe('toggleTooltip', () => {
        it('should setState exactly', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<EditJobInfo {...props}/>);
            container.setState({ tooltipOpen: false });
            container.instance().toggleTooltip();

            // Assert
            expect(container.state().tooltipOpen).toEqual(true);
        });
    });
});