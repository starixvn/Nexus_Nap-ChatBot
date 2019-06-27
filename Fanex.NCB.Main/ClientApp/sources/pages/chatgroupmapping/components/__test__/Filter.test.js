import React from 'react';
import { Filter } from '../Filter';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("Filter", () => { 
    describe("constructor", () => { 
        it("init state", () => { 
            // Arrange
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };

            // Act
            let filter = new Filter(props);

            // Assert
            expect(filter.state.botNames.value).toEqual("");
            expect(filter.state.recipientTypes.value).toEqual("");
            expect(filter.state.recipient.value).toEqual("");
            expect(filter.state.mappingStatuses.value).toEqual("");
        });
    });

    describe("getDerivedStateFromProps", () => {
        it("state.settingData.botNames empty and nexProps.settingData.botNames exist", () => {
            // Arrange
            let nextProps = {
                settingData: {
                    botNames: [{ id: 1 }, { id: 2 }],
                    recipientTypes: [{ id: 1 }, { id: 2 }],
                    mappingStatuses: [{ id: 1 }, { id: 2 }],
                    licenseeStatuses: [{ id: 1 }, { id: 2 }],
                }
            };
            let prevState = {
                settingData: {
                    botNames: []
                }
            };

            let expectedState = {
                botNames: {
                    value: "1,2"
                },
                recipientTypes: {
                    value: "1,2"
                },
                recipient: {
                    value: ""
                },
                licenseeStatuses: {
                    value: "1,2"
                },
                mappingStatuses: {
                    value: "1,2"
                },
                settingData: {
                    botNames: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
                    recipientTypes: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
                    mappingStatuses: [{ checked: true, id: 1 }, { checked: true, id: 2 }],
                    licenseeStatuses: [{ checked: true, id: 1 }, { checked: true, id: 2 }]
                }
            };

            // Act
            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toEqual(expectedState);
        });

        it("state.settingData.botNames not empty", () => {
            // Arrange
            let nextProps = {
                settingData: {
                    botNames: null
                }
            };
            let prevState = {
                settingData: {
                    botNames: []
                }
            };

            // Act
            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toBeNull();
        });

        it("nextProps.settingData.botNames does not exist", () => {
            // Arrange
            let nextProps = {};
            let prevState = {
                settingData: {
                    botNames: [1]
                }
            };

            // Act
            var result = Filter.getDerivedStateFromProps(nextProps, prevState);

            // Assert
            expect(result).toBeNull();
        });
    });
   
    describe("onChange", () => { 
        it("when called - setState", () => { 
            // Arrange
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.setState = jest.fn();
            let fieldName = "field name";
            let fieldValue = 123; 

            // Act
            filter.onChange(fieldName, fieldValue);

            // Assert
            expect(filter.setState).toHaveBeenCalledWith({[fieldName]: fieldValue});
        });
    });

    describe("getParams", () => { 
        it("get params from state", () => { 
            // Arrange
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.state = {
                botNames: {
                    value: "bot name"
                },
                recipientTypes: {
                    value: "recipient types"
                },
                recipient: {
                    value: "recipient 1"
                },
                mappingStatuses: {
                    value: "mapping statuses"
                },
                licenseeStatuses: {
                    value: "licensee statuses"
                }
            };
            let expectedParams = {
                botNames: "bot name",
                recipientTypeIds: "recipient types",
                recipientName: "recipient 1",
                mappingStatusIds: "mapping statuses",
                licenseeStatusIds: "licensee statuses",
                pageIndex: 1,
                pageSize: 5
            };

            // Act
            let result = filter.getParams();

            // Assert
            expect(result).toEqual(expectedParams);
        });
    });

    describe("submitHandler", () => { 
        it("when submit - call props functions", () => { 
            // Arrange
            let props = {
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);            
            let params = "some params";
            filter.getParams = jest.fn(() => { return params; });            
            let event = { preventDefault: jest.fn() };

            // Act
            filter.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(props.setGridParams).toHaveBeenCalledWith(params);
            expect(props.getGridData).toHaveBeenCalledWith(params);
        });
    });


    describe("render", () => { 
        it("render correctly", () => { 
            // Arrange 
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn(),
                settingData: {
                    botNames: [],
                    recipientTypes: [],
                    mappingStatuses: [],
                    licenseeStatuses: []
                }
            };

            // Act
            let filter = (new ShallowRenderer()).render(<Filter {...props} />);

            // Assert
            expect(filter).toMatchSnapshot();
        });
    });

    describe("checkValidParams", () => {
        it("when all values != empty - return true", () => {
            // Arrange                        
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);           
            filter.state = {
                botNames: { value: "a" },
                recipientTypes: { value: "a" },
                mappingStatuses: { value: "a" },
                licenseeStatuses: { value: "a" }
            };

            // Act
            var result = filter.checkValidParams();

            // Assert
            expect(result).toEqual(true);
        });

        it("when botNames empty - return false", () => {
            // Arrange                        
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.state = {
                botNames: { value: "" },
                recipientTypes: { value: "a" },
                mappingStatuses: { value: "a" },
                licenseeStatuses: { value: "a" }
            };

            // Act
            var result = filter.checkValidParams();

            // Assert
            expect(result).toEqual(false);
        });

        it("when recipientTypes empty - return false", () => {
            // Arrange                        
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.state = {
                botNames: { value: "a" },
                recipientTypes: { value: "" },
                mappingStatuses: { value: "a" },
                licenseeStatuses: { value: "a" }
            };

            // Act
            var result = filter.checkValidParams();

            // Assert
            expect(result).toEqual(false);
        });

        it("when mappingStatuses empty - return false", () => {
            // Arrange                        
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.state = {
                botNames: { value: "a" },
                recipientTypes: { value: "a" },
                mappingStatuses: { value: "" },
                licenseeStatuses: { value: "a" }
            };

            // Act
            var result = filter.checkValidParams();

            // Assert
            expect(result).toEqual(false);
        });

        it("when licenseeStatuses empty - return false", () => {
            // Arrange                        
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);
            filter.state = {
                botNames: { value: "a" },
                recipientTypes: { value: "a" },
                mappingStatuses: { value: "a" },
                licenseeStatuses: { value: "" }
            };

            // Act
            var result = filter.checkValidParams();

            // Assert
            expect(result).toEqual(false);
        });
    });
});