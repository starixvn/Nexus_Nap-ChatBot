import React from 'react';
import { UpdatePopup } from '../UpdatePopup';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("UpdatePopup", () => {
    const defaultProps = {
        updatePopupData: {
            editingItem: {
                groupID: 1,
                conversationID: 2,
                recipientTypeId: { value: 3 },
                recipientIds: { value: "4,5" },
                remark: { value: "remark 1" }
            }
        }
    };

    describe("constructor", () => {
        it("get state from prop", () => {
            // Arrange
            let props = {
                ...defaultProps
            };

            // Act
            let updatePopup = new UpdatePopup(props);

            // Assert
            expect(updatePopup.state.groupID).toEqual(props.updatePopupData.editingItem.groupID);
            expect(updatePopup.state.conversationID).toEqual(props.updatePopupData.editingItem.conversationID);
            expect(updatePopup.state.recipientTypeId).toEqual(props.updatePopupData.editingItem.recipientTypeId);
            expect(updatePopup.state.recipientIds).toEqual(props.updatePopupData.editingItem.recipientIds);
            expect(updatePopup.state.remark).toEqual(props.updatePopupData.editingItem.remark);
            expect(updatePopup.state.isSummited).toEqual(false);
        });
    });

    describe("render", () => {
        it("when isShowed is false - return null", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    ...defaultProps.updatePopupData,
                    isShowed: false
                }
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            let result = updatePopup.render();

            // Assert
            expect(result).toBe(null);
        });

        it("when filteredRecipients is null - return null", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    ...defaultProps.updatePopupData,
                    isShowed: true
                },
                filteredRecipients: null
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            let result = updatePopup.render();

            // Assert
            expect(result).toBe(null);
        });

        it("when SuccessMsg shown", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    ...defaultProps.updatePopupData,
                    isShowed: true
                },
                filteredRecipients: [],
                settingData: {},
                updateData: { addedresult: true }
            };

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props} />);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when SuccessMsg is not shown", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    ...defaultProps.updatePopupData,
                    isShowed: true
                },
                filteredRecipients: [],
                settingData: {},
                updateData: { addedresult: false }
            };

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props} />);

            // Assert
            expect(result).toMatchSnapshot();
        });
    });

    describe("submitHandler", () => {
        it("when not valid params - not call update", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            let event = { preventDefault: jest.fn() };
            updatePopup.setState = jest.fn();
            updatePopup.checkValidParams = jest.fn(() => { return false;});
            updatePopup.state.isSummited = false;

            // Act
            updatePopup.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(updatePopup.setState).toHaveBeenCalledWith({ isSummited: true });
        });

        it("when valid params - call update", (done) => {
            // Arrange
            let response = {};
            let props = {
                ...defaultProps,
                update: jest.fn(() => Promise.resolve(response))
            };
            let updatePopup = new UpdatePopup(props);
            let event = { preventDefault: jest.fn() };
            updatePopup.setState = jest.fn();
            updatePopup.checkValidParams = jest.fn(() => { return true; });
            updatePopup.state.isSummited = true;
            let params = "params";
            updatePopup.getParams = jest.fn(() => { return params; });
            updatePopup.closePopup = jest.fn();

            // Act
            updatePopup.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
            expect(updatePopup.setState).not.toHaveBeenCalled();
            expect(props.update).toHaveBeenCalledWith(params);
            props.update().then(() => {
                expect(updatePopup.closePopup).toHaveBeenCalled();
                done();
            });
        });
    });

    describe("getParams", () => {
        it("get params from state", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.state = {
                groupID: 1,
                conversationID: 2,
                recipientTypeId: {
                    value: 3
                },
                recipientIds: { value: "4,5" },
                remark: { value: "remark" }
            };            
            let expectedParams = {
                groupID: 1,
                conversationId: 2,
                recipientTypeId: 3,
                recipientIds: "4,5",
                remark: "remark"
            };

            // Act
            let result = updatePopup.getParams();

            // Assert
            expect(result).toEqual(expectedParams);
        });
    });

    describe("checkValidParams", () => {
        it("when recipientTypeId valid and recipientIds valid and  remark valid - return true", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.state = {
                recipientTypeId: { isValid: true },
                recipientIds: { isValid: true },
                remark: { isValid: true }
            };    

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(true);
        });

        it("when recipientTypeId invalid and recipientIds valid and  remark valid - return false", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.state = {
                recipientTypeId: { isValid: false },
                recipientIds: { isValid: true },
                remark: { isValid: true }
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });

        it("when recipientTypeId valid and recipientIds invalid and  remark valid - return false", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.state = {
                recipientTypeId: { isValid: true },
                recipientIds: { isValid: false },
                remark: { isValid: true }
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });

        it("when recipientTypeId valid and recipientIds valid and  remark invalid - return false", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.state = {
                recipientTypeId: { isValid: true },
                recipientIds: { isValid: true },
                remark: { isValid: false }
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });
    });

    describe("onChange", () => {
        it("when called - setState", () => {
            // Arrange
            let props = {
                ...defaultProps
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.setState = jest.fn();
            let fieldName = "field name";
            let fieldValue = 123;

            // Act
            updatePopup.onChange(fieldName, fieldValue);

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({ [fieldName]: fieldValue });
        });
    });

    describe("closePopup", () => {
        it("call closeUpdatePopupHandler after 1000ms", (done) => {
            // Arrange
            let props = {
                ...defaultProps,
                closeUpdatePopupHandler: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            updatePopup.closePopup();

            // Assert
            setTimeout(() => {
                expect(props.closeUpdatePopupHandler).toHaveBeenCalled();
                done();
            }, 1000);
        });
    });

    describe("filterRecipients", () => {
        it("call filterRecipients and update state", () => {
            // Arrange
            let props = {
                ...defaultProps,
                settingData: {
                    recipients: [{}]
                },
                filterRecipients: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);
            updatePopup.setState = jest.fn();
            let recipientTypeId = 1;

            // Act
            updatePopup.filterRecipients(recipientTypeId);

            // Assert
            expect(props.filterRecipients).toHaveBeenCalledWith(props.settingData.recipients, recipientTypeId);
            expect(updatePopup.setState).toHaveBeenCalledWith({ recipientIds: {value: '', isValid: false}});
        });
    });
});