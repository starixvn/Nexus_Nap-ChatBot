import React from 'react';
import { Filter } from '../Filter';
import { shallow } from 'enzyme';

describe('Filter', () => {
    afterAll(() => {
        jest.clearAllMocks();
    });

    //    describe('constructor', () => {
    //        it('init state', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };

    //            // Act
    //            let filter = new Filter(props);

    //            // Assert
    //            expect(filter.state.botIds.value).toEqual('');
    //            expect(filter.state.recipientTypes.value).toEqual('');
    //            expect(filter.state.recipient.value).toEqual('');
    //            expect(filter.state.mappingStatuses.value).toEqual('');
    //        });
    //    });

    //    describe('getDerivedStateFromProps', () => {
    //        it('state.settingData.botNames empty and nexProps.settingData.botNames exist', () => {
    //            // Arrange
    //            let nextProps = {
    //                settingData: {
    //                    bots: [{ id: 1 }, { id: 2 }],
    //                    recipientTypes: [{ id: 1 }, { id: 2 }],
    //                    mappingStatuses: [{ id: 1 }, { id: 2 }],
    //                    licenseeStatuses: [{ id: 1 }, { id: 2 }],
    //                }
    //            };
    //            let prevState = {
    //                settingData: {
    //                    bots: []
    //                }
    //            };

    //            let expectedState = {
    //                botIds: {
    //                    value: '1,2'
    //                },
    //                recipientTypes: {
    //                    value: '1,2'
    //                },
    //                recipient: {
    //                    value: ''
    //                },
    //                licenseeStatuses: {
    //                    value: '1,2'
    //                },
    //                mappingStatuses: {
    //                    value: '1,2'
    //                },
    //                settingData: {
    //                    bots: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
    //                    recipientTypes: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
    //                    mappingStatuses: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
    //                    licenseeStatuses: [{ checked: true, id: 1 }, { checked: true, id: 2 }]
    //                }
    //            };

    //            // Act
    //            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

    //            // Assert
    //            expect(result).toEqual(expectedState);
    //        });

    //        it('state.settingData.botNames not empty', () => {
    //            // Arrange
    //            let nextProps = {
    //                settingData: {
    //                    bots: null
    //                }
    //            };
    //            let prevState = {
    //                settingData: {
    //                    bots: []
    //                }
    //            };

    //            // Act
    //            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

    //            // Assert
    //            expect(result).toBeNull();
    //        });

    //        it('nextProps.settingData.botNames does not exist', () => {
    //            // Arrange
    //            let nextProps = {};
    //            let prevState = {
    //                settingData: {
    //                    bots: [1]
    //                }
    //            };

    //            // Act
    //            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

    //            // Assert
    //            expect(result).toBeNull();
    //        });
    //    });

    //    describe('onChange', () => {
    //        it('when called - setState', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.setState = jest.fn();
    //            let fieldName = 'field name';
    //            let fieldValue = 123;

    //            // Act
    //            filter.onChange(fieldName, fieldValue);

    //            // Assert
    //            expect(filter.setState).toHaveBeenCalledWith({ [fieldName]: fieldValue });
    //        });
    //    });

    //describe('getParams', () => {
    //    it('should return correctly params', () => {
    //        // Arrange
    //        const expected = {
    //            channelIds: '1',
    //            botIds: '1',
    //            recipientTypeIds: '1',
    //            recipient: '1',
    //            groupName: 'ncb',
    //            mappingStatusIds: '1',
    //            licenseeStatusIds: '1',
    //            pageIndex: 1,
    //            pageSize: 10
    //        };
    //        const container = shallow(<Filter />);
    //        container.setState({
    //            channelIds: '1',
    //            botIds: '1',
    //            recipientTypeIds: '1',
    //            recipient: { value: '1' },
    //            groupName: { value: 'ncb' },
    //            mappingStatusIds: '1',
    //            licenseeStatusIds: '1'
    //        });

    //        // Act
    //        const actual = container.instance().getParams();

    //        // Assert
    //        expect(actual).toEqual(expected);
    //    });
    //});

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

    //    describe('checkValidParams', () => {
    //        it('when all values != empty - return true', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.state = {
    //                botIds: { value: '1,2' },
    //                recipientTypes: { value: 'a' },
    //                mappingStatuses: { value: 'a' },
    //                licenseeStatuses: { value: 'a' }
    //            };

    //            // Act
    //            var result = filter.checkValidParams();

    //            // Assert
    //            expect(result).toEqual(true);
    //        });

    //        it('when botNames empty - return false', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.state = {
    //                botIds: { value: '' },
    //                recipientTypes: { value: 'a' },
    //                mappingStatuses: { value: 'a' },
    //                licenseeStatuses: { value: 'a' }
    //            };

    //            // Act
    //            var result = filter.checkValidParams();

    //            // Assert
    //            expect(result).toEqual(false);
    //        });

    //        it('when recipientTypes empty - return false', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.state = {
    //                botIds: { value: 'a' },
    //                recipientTypes: { value: '' },
    //                mappingStatuses: { value: 'a' },
    //                licenseeStatuses: { value: 'a' }
    //            };

    //            // Act
    //            var result = filter.checkValidParams();

    //            // Assert
    //            expect(result).toEqual(false);
    //        });

    //        it('when mappingStatuses empty - return false', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.state = {
    //                botIds: { value: 'a' },
    //                recipientTypes: { value: 'a' },
    //                mappingStatuses: { value: '' },
    //                licenseeStatuses: { value: 'a' }
    //            };

    //            // Act
    //            var result = filter.checkValidParams();

    //            // Assert
    //            expect(result).toEqual(false);
    //        });

    //        it('when licenseeStatuses empty - return false', () => {
    //            // Arrange
    //            let props = {
    //                getSettings: jest.fn(),
    //                filterRecipients: jest.fn()
    //            };
    //            let filter = new Filter(props);
    //            filter.state = {
    //                botIds: { value: 'a' },
    //                recipientTypes: { value: 'a' },
    //                mappingStatuses: { value: 'a' },
    //                licenseeStatuses: { value: '' }
    //            };

    //            // Act
    //            var result = filter.checkValidParams();

    //            // Assert
    //            expect(result).toEqual(false);
    //        });
    //    });
});