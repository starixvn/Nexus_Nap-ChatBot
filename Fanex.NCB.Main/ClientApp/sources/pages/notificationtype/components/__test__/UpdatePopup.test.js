import React from 'react';
import {UpdatePopup} from '../UpdatePopup';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('UpdatePopup', () => {
    const defaultProps = {
        updatePopupData: {
            editingItem: {
                notificationTypeName: {},
                recipientTypeIds:{},
                systemId:{}
            }
        }
    };

    describe("constructor", () => {
        it("init state", () => {
            // Arrange           
            let props = {
                updatePopupData: {
                    editingItem: {
                        notificationTypeId: 1,
                        notificationTypeName: {value: "abc", isValid: true},
                        recipientTypeIds: {value: 2, isValid: true},
                        systemId: {value: 3, isValid: true}
                    }
                }
            };            

            // Act
            let updatePopup = new UpdatePopup(props);

            // Assert
            expect(updatePopup.state.notificationTypeId).toEqual(1);
            expect(updatePopup.state.notificationTypeName.value).toEqual("abc");
            expect(updatePopup.state.notificationTypeName.isValid).toBe(true);
            expect(updatePopup.state.recipientTypeIds.value).toEqual(2);
            expect(updatePopup.state.recipientTypeIds.isValid).toBe(true);
            expect(updatePopup.state.systemId.value).toEqual(3);
            expect(updatePopup.state.systemId.isValid).toBe(true);
            expect(updatePopup.state.isSummited).toBe(false);
        });
    });

    describe("mount", () => {
        it("componentWillMount", () => {
            // Arrange
            let props = {
                updatePopupData: {
                    editingItem: {
                        notificationTypeName: {},
                        notificationTypeId: {},
                        recipientTypeIds:{},
                        systemId:{}
                    }
                },
                getSettings: jest.fn()
            };
            let updatePopup = new UpdatePopup(props);

            // Act
            updatePopup.componentWillMount();

            // Assert
            expect(props.getSettings).toHaveBeenCalled();
        });
    });

    describe("submitHandler", () => {        

        it("call prevenDefault", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.setState = jest.fn();
            let event = {preventDefault: jest.fn()};
            
            // Act
            updatePopup.submitHandler(event);

            // Assert
            expect(event.preventDefault).toHaveBeenCalled();
        });

        it("when isSummited is false - set isSummited to true", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state.isSummited = false;
            updatePopup.setState = jest.fn();
            
            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({isSummited: true});
        });

        it("when isSummited is false - not update isSummited", () => {
            // Arrange            
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state.isSummited = true;
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(updatePopup.setState).not.toHaveBeenCalledWith({isSummited: true});
        });

        it("when valid params - call props.update then call closePopup", (done) => {
             // Arrange    
             let response = {};
             let props =  {
                ...defaultProps,
                update: jest.fn(() => Promise.resolve(response))
            };
             let updatePopup = new UpdatePopup(props);

             let params = {data1: []};
             updatePopup.setState = jest.fn();
             updatePopup.closePopup = jest.fn();
             updatePopup.getParams = jest.fn(() => {return params;});
             updatePopup.checkValidParams = jest.fn(() => {return true;});
 
             // Act
             updatePopup.submitHandler({preventDefault: jest.fn()});

             // Assert
             expect(props.update).toHaveBeenCalledWith(params, props.updatePopupData.isNew);
             props.update().then(() => {
                expect(updatePopup.closePopup).toHaveBeenCalled();
                done();
             });
        });

        it("when not valid params - do not call props.update", () => {
            // Arrange    
            let response = {};
            let props =  {
               ...defaultProps,
               update: jest.fn(() => Promise.resolve(response))
           };
            let updatePopup = new UpdatePopup(props);
            
            updatePopup.setState = jest.fn();
            updatePopup.checkValidParams = jest.fn(() => {return false;});
           
            // Act
            updatePopup.submitHandler({preventDefault: jest.fn()});

            // Assert
            expect(props.update).not.toHaveBeenCalled();
        });
    });

    describe("get Params", () => {
        it("get params from state", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeId: 1,
                notificationTypeName: {value: "some name"},
                recipientTypeIds: {value: "2"},
                systemId: { value: 3 }                
            };
            let params = {
                notificationTypeId: 1,
                notificationTypeName: "some name",
                recipientTypeIds: "2",
                systemId: 3
            };

            // Act
            let result = updatePopup.getParams();

            // Assert
            expect(result).toEqual(params);
        });
    });

    describe("checkValidParams", () => {
        it("valid params", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: {isValid: true},
                systemId: {isValid: true}
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(true);
        });

        it("invalid notificationTypeName - return invalid", () => {
             // Arrange
             let updatePopup = new UpdatePopup(defaultProps);
             updatePopup.state = {
                 isSummited: true,
                 notificationTypeName: {isValid: false},
                 recipientTypeIds: {isValid: true},
                 systemId: {isValid: true}
             };

             // Act
             let result = updatePopup.checkValidParams();

             // Assert
             expect(result).toBe(false);
        });

        it("invalid recipientTypeId - return invalid", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: {isValid: false},
                systemId: {isValid: true}
            };

            // Act
            let result = updatePopup.checkValidParams();

            // Assert
            expect(result).toBe(false);
        });

        it("invalid systemId - return invalid", () => {
            // Arrange
            let updatePopup = new UpdatePopup(defaultProps);
            updatePopup.state = {
                isSummited: true,
                notificationTypeName: {isValid: true},
                recipientTypeIds: {isValid: true},
                systemId: {isValid: false}
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
            let updatePopup = new UpdatePopup(defaultProps);
            let fieldName = "field name";
            let fieldValue = 123;
            updatePopup.setState = jest.fn();

            // Act
            updatePopup.onChange(fieldName, fieldValue);

            // Assert
            expect(updatePopup.setState).toHaveBeenCalledWith({[fieldName]: fieldValue});
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

    describe("render", () => {
        const defaultProps = {
            updatePopupData: {
                editingItem: {
                    notificationTypeName: { value: "abc", isValid: true },
                    recipientTypeIds:{ value: "2", isValid: true },
                    systemId:{ value: 3, isValid: true }
                },
                popupTitle: "title",                    
                isShowed: true,
                successMsg: "sucess"                   
            },
            closeUpdatePopupHandler: jest.fn(),
            getSettings: jest.fn(),
            settingData: {
                recipientTypes: [],
                notificationSystems: []
            }
        };

        it("when isShowed false - return null", () => {
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

        it("when isShowed true and NOT showSucessMsg", () => {
            // Arrange
            let props = {
                ...defaultProps,
                updateData: null
            };    

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when isShowed true and showSucessMsg", () => {
             // Arrange
             let props = {
                ...defaultProps,
                updateData: {
                    addedresult: true
                }
            };    

            // Act
            let result = (new ShallowRenderer()).render(<UpdatePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });
    });
});