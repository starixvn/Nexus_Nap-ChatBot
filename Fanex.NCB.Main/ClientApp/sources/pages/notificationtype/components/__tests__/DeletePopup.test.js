import React from 'react';
import {DeletePopup} from '../DeletePopup';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('DeletePopup', () => {   
    describe('confirmHandler', () => {
        it('when confirm is yes', (done) => {
           // Arrange
           const response = { data: [] };
           let props = {
               deleteItem: jest.fn((id) => Promise.resolve(response)),
               closeDeletePopupHandler: jest.fn(),
               deletePopupData: { notificationTypeId: 1}
           };
            let deletePopup = new DeletePopup(props);
            deletePopup.closePopup = jest.fn();

           // Act
           deletePopup.confirmHandler(true);

           // Assert
           expect(props.deleteItem).toHaveBeenCalledWith(1);           
           props.deleteItem().then(() =>{
               expect(deletePopup.closePopup).toHaveBeenCalled();
            done();
           });
        });

        it('when confirm is no', () => {
           // Arrange
           let props = {
                deleteItem: jest.fn(),
                closeDeletePopupHandler: jest.fn()
           };
           let deletePopup = new DeletePopup(props);

           // Act
           deletePopup.confirmHandler(false);

           // Assert
           expect(props.closeDeletePopupHandler).toHaveBeenCalled();
        });
    });

    describe("render", () => {
        it("when no deleteData - return null", () => {
            // Arrange
            let props = {
                deletePopupData: {
                    content: "some content"
                },
                deleteData: { deleteResult: null },
                closeDeletePopupHandler: jest.fn()
            };

            // Act
            let result = (new ShallowRenderer()).render(<DeletePopup {...props} />);

            // Assert
            expect(result).toMatchSnapshot();
        });

        it("when has deleteData", () => {
            // Arrange
            let props = {
                deletePopupData: {
                    content: "some content"
                },
                deleteData: {
                    deleteResult: {
                        isSuccessful: true,
                        successMsg: "msg1",
                        message: "msg2"
                    }
                },
                closeDeletePopupHandler: jest.fn()
            };
          
            // Act
            let result = (new ShallowRenderer()).render(<DeletePopup {...props}/>);

            // Assert
            expect(result).toMatchSnapshot();
        });
    });     
});