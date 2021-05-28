import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';

describe('Filter', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('componentDidMount', () => {
        it('should call correctly functions', () => {
            // Arrange
            const props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            container.componentDidMount();

            // Assert
            expect(props.getSettings).toHaveBeenCalled();
            expect(props.filterRecipients).toHaveBeenCalledWith([], 0);
        });
    });

    describe('getDerivedStateFromProps', () => {
        it('should update correctly state when prevState.settings.bots is empty and nexProps.settings.bots has data', () => {
            // Arrange
            const nextProps = {
                settings: {
                    channelIds: '1', botIds: '1',
                    channels: [{ id: 1 }],
                    bots: [{ id: 1 }, { id: 2 }],
                    recipientTypes: [{ id: 1, name: 'licensee' }],
                    mappingStatuses: [{ id: 1, name: 'mapped' }],
                    licenseeStatuses: [{ id: 1, name: 'active' }]
                }
            };
            const prevState = {
                settings: { bots: [] }
            };
            const expectedState = {
                channelIds: '1', botIds: '1',
                recipientTypeIds: '1',
                recipient: {
                    value: ''
                },
                licenseeStatusIds: '1',
                mappingStatusIds: '1',
                settings: {
                    channels: [{ id: 1 }],
                    bots: [{ id: 1 }, { id: 2 }],
                    recipientTypes: [{ id: 1, text: 'licensee', checked: true, isLeaf: true }],
                    mappingStatuses: [{ id: 1, text: 'mapped', checked: true, isLeaf: true }],
                    licenseeStatuses: [{ id: 1, text: 'active', checked: true, isLeaf: true }]
                }
            };

            // Act
            const result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toEqual(expectedState);
        });

        it('return null when prevState.settings.bots has data', () => {
            // Arrange
            const nextProps = {
                settings: { bots: null }
            };
            const prevState = {
                settings: { bots: [{ id: 1 }] }
            };

            // Act
            const result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toBeNull();
        });

        it('nextProps.settings.botNames does not exist', () => {
            // Arrange
            const nextProps = {
                settings: {}
            };
            const prevState = {
                settings: { bots: [] }
            };

            // Act
            const result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toBeNull();
        });
    });

    describe('onChange', () => {
        it('should update correctly state', () => {
            // Arrange
            const fieldName = 'field name',
                fieldValue = 123;
            const container = shallow(<Filter />);
            container.setState = jest.fn();

            // Act
            container.instance().onChange(fieldName, fieldValue);

            // Assert
            expect(container.setState).toHaveBeenCalledWith({ [fieldName]: fieldValue });
        });
    });

    describe('getParams', () => {
        it('should return correctly params', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [], mappingStatuses: [], licenseeStatuses: []
                }
            };
            const expected = {
                channelIds: '1', botIds: '1',
                recipientTypeIds: '', recipientName: '', groupName: '',
                mappingStatusIds: '', licenseeStatusIds: '',
                pageIndex: 1, pageSize: 20
            };
            const container = shallow(<Filter {...props} />);

            // Act
            const actual = container.instance().getParams();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('checkValidParams', () => {
        it('should return true when all values is valid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [{ id: 1 }],
                    mappingStatuses: [{ id: 1 }],
                    licenseeStatuses: [{ id: 1 }]
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeTruthy();
        });

        it('should return false when channelIds is invalid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [{ id: 1 }],
                    mappingStatuses: [{ id: 1 }],
                    licenseeStatuses: [{ id: 1 }]
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeFalsy();
        });

        it('should return false when botIds is invalid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [{ id: 1 }],
                    mappingStatuses: [{ id: 1 }],
                    licenseeStatuses: [{ id: 1 }]
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeFalsy();
        });

        it('should return false when recipientTypeIds is invalid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [],
                    mappingStatuses: [{ id: 1 }],
                    licenseeStatuses: [{ id: 1 }]
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeFalsy();
        });

        it('should return false when mappingStatusIds is invalid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [{ id: 1 }],
                    mappingStatuses: [],
                    licenseeStatuses: [{ id: 1 }]
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeFalsy();
        });

        it('should return false when licenseeStatusIds is invalid', () => {
            // Arrange
            const props = {
                settings: {
                    channelIds: '1', botIds: '1',
                    bots: [{ botId: 1, botName: 'ncb' }],
                    recipientTypes: [{ id: 1 }],
                    mappingStatuses: [{ id: 1 }],
                    licenseeStatuses: []
                }
            };
            const container = shallow(<Filter {...props} />).instance();

            // Act
            const result = container.checkValidParams();

            // Assert
            expect(result).toBeFalsy();
        });
    });

    describe('submitHandler', () => {
        it('should call exactly functions', () => {
            // Arrange
            const expectedParams = {
                channelIds: '1',
                botIds: '1',
                recipientTypeIds: '1',
                recipient: '1',
                groupName: 'ncb',
                mappingStatusIds: '1',
                licenseeStatusIds: '1',
                pageIndex: 1,
                pageSize: 10
            };
            const props = {
                setGridParams: jest.fn(),
                getGridData: jest.fn()
            };
            const container = shallow(<Filter {...props} />).instance();
            container.getParams = jest.fn(() => expectedParams);

            // Act
            container.submitHandler({ preventDefault: jest.fn() });

            // Assert
            expect(props.setGridParams).toHaveBeenCalledWith(expectedParams);
            expect(props.getGridData).toHaveBeenCalledWith(expectedParams);
        });
    });

    //    describe('render', () => {
    //        it('render correctly', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn(),
    //                settingData: {
    //                    bots: [],
    //                    recipientTypes: [],
    //                    mappingStatuses: [],
    //                    licenseeStatuses: []
    //                }
    //            };

    //            // Act
    //            let filter = (new ShallowRenderer()).render(<Filter {...props} />);

    //            // Assert
    //            expect(filter).toMatchSnapshot();
    //        });
    //    });
});