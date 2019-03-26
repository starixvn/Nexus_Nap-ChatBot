import React from 'react';
import { Filter } from '../Filter';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("Filter", () => { 
    describe("constructor", () => { 
        it("init state", () => { 
            // Arrange
            let props = {};

            // Act
            let filter = new Filter(props);

            // Assert
            expect(filter.state.botNames.value).toEqual("");
            expect(filter.state.recipientTypes.value).toEqual("");
            expect(filter.state.recipient.value).toEqual("");
            expect(filter.state.mappingStatuses.value).toEqual("");
        });
    });

    describe("mount", () => { 
        it("componentWillMount", () => { 
            // Arrange 
            let props = {
                getSettings: jest.fn(),
                filterRecipients: jest.fn()
            };
            let filter = new Filter(props);

            // Act
            filter.componentWillMount();

            // Assert
            expect(props.getSettings).toHaveBeenCalled();
            expect(props.filterRecipients).toHaveBeenCalledWith([], 0);
        });
    });

    describe("onChange", () => { 
        it("when called - setState", () => { 
            // Arrange
            let props = {};
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
            let props = {};
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
                }
            };
            let expectedParams = {
                botNames: "bot name",
                recipientTypeIds: "recipient types",
                recipientName: "recipient 1",
                mappingStatusIds: "mapping statuses",
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
                getGridData: jest.fn()
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
                    botNames: "bot name",
                    recipientTypes: "recipient Types",
                    mappingStatuses: "mapping Statuses"
                }
            };

            // Act
            let filter = (new ShallowRenderer()).render(<Filter {...props} />);

            // Assert
            expect(filter).toMatchSnapshot();
        });
    });
});