import React from 'react';
import { JobInfo } from '../JobInfo';
import { shallow } from 'enzyme';

describe('JobInfo', () => {

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

    describe('handleChangeEditMode', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                setJobInfoFlag: jest.fn()
            };

            // Act
            const container = shallow(<JobInfo {...props}/>);
            container.instance().handleChangeEditMode();

            // Assert
            expect(props.setJobInfoFlag).toHaveBeenCalledWith({ isEdit: true });
        });
    });

    describe('render', () => {
        it('should render tag p exactly when status is done', () => {
            // Arrange
            const props = {
                jobInfo: {
                    status: 'Done',
                    id: 4,
                    previousMaxMessageId: 1,
                    batchSize: 10,
                    testingGroup: 'test'
                }
            }

            // Act
            const container = shallow(<JobInfo {...props}/>);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find('p').length).toBe(8);
            expect(render.find('p').at(1).props().className).toBe('job-info-value status-done');
        });

        it('should render tag p exactly when status is not done', () => {
            // Arrange
            const props = {
                jobInfo: {
                    status: 'Running',
                    id: 2,
                    previousMaxMessageId: 1,
                    batchSize: 10,
                    testingGroup: 'test'
                }
            }

            // Act
            const container = shallow(<JobInfo {...props}/>);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find('p').length).toBe(8);
            expect(render.find('p').at(1).props().className).toBe('job-info-value status-running');
        });
    });
});