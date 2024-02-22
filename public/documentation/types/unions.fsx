type ContactInfo =
    | PhoneNumber of string
    | EmailAddress of string

let contact contactInfo =
    match contactInfo with
    | PhoneNumber number -> $"Sending a text message to {number}"
    | EmailAddress email -> $"Sending an email to {email}"

let contactViaEmail = contact (EmailAddress "johndoe@site.com")
printfn "%s" contactViaEmail
